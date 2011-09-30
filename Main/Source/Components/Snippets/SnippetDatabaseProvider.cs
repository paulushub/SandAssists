using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;

using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetDatabaseProvider : SnippetProvider
    {
        #region Private Fields

        private const string DatabaseName = "CodeSnippets.edb";

        private const string WorkingDirectory = "CodeSnippets";

        /// <summary>
        /// Name of the table that will store the snippets.
        /// </summary>
        private const string TableName = "Snippets";

        /// <summary>
        /// Columnid of the snippet group.
        /// </summary>
        private JET_COLUMNID _columnGroup;

        /// <summary>
        /// Columnid of the snippet ID.
        /// </summary>
        private JET_COLUMNID _columnId;

        /// <summary>
        /// Columnid of the snippet language.
        /// </summary>
        private JET_COLUMNID _columnLang;

        /// <summary>
        /// Columnid of the snippet text.
        /// </summary>
        private JET_COLUMNID _columnText;

        private int         _itemCount;
        private bool        _isRegistering;

        private string      _databaseDir;

        private Table       _databaseTable;
        private Session     _databaseSession;
        private Instance    _databaseInstance;
        private Transaction _databaseTransaction;

        #endregion

        #region Constructors and Destructor

        public SnippetDatabaseProvider(Type componentType,
            MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
        }

        static SnippetDatabaseProvider()
        {
            Globals.Init();
        }

        #endregion

        #region Public Properties

        public override int Count
        {
            get
            {
                return _itemCount;
            }
        }

        public override bool IsMemory
        {
            get
            {
                return false;
            }
        }

        public override SnippetStorage Storage
        {
            get
            {
                return SnippetStorage.Database;
            }
        }

        public override IList<SnippetItem> this[SnippetInfo info]
        {
            get
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info",
                        "The snippet information cannot be null (or Nothing).");
                }
                if (_itemCount <= 0 || _databaseInstance == null
                    || _databaseSession == null || _databaseTable == null)
                {
                    return null;
                }

                List<SnippetItem> listInfo = null;
  
                string exampleId = info.ExampleId;
                string snippetId = info.SnippetId;

                // We are about to set up an index range on the name index.
                Api.JetSetCurrentIndex(_databaseSession, _databaseTable, "identifier");

                Api.MakeKey(_databaseSession, _databaseTable, exampleId, 
                    Encoding.Unicode, MakeKeyGrbit.NewKey);
                Api.MakeKey(_databaseSession, _databaseTable, snippetId, 
                    Encoding.Unicode, MakeKeyGrbit.None);
                if (Api.TrySeek(_databaseSession, _databaseTable, SeekGrbit.SeekGE))
                {
                    Api.MakeKey(_databaseSession, _databaseTable, exampleId, 
                        Encoding.Unicode, MakeKeyGrbit.NewKey);
                    Api.MakeKey(_databaseSession, _databaseTable, snippetId, 
                        Encoding.Unicode, MakeKeyGrbit.None);
                    if (Api.TrySetIndexRange(_databaseSession, _databaseTable, 
                        SetIndexRangeGrbit.RangeUpperLimit | SetIndexRangeGrbit.RangeInclusive))
                    {
                        listInfo = new List<SnippetItem>();
                        do
                        {
                            string snippetLang = Api.RetrieveColumnAsString(
                                _databaseSession, _databaseTable, _columnLang);
                            string snippetText = Api.RetrieveColumnAsString(
                                _databaseSession, _databaseTable, _columnText);
                            listInfo.Add(new SnippetItem(snippetLang, snippetText));
                        }
                        while (Api.TryMoveNext(_databaseSession, _databaseTable));
                    }
                }

                return listInfo;
            }
        }

        #endregion

        #region Public Methods

        public override bool StartRegister(bool clearExisting)
        {
            if (_isRegistering)
            {
                return false;
            }

            try
            {
                string workingDir = Path.GetFullPath(WorkingDirectory);
                if (clearExisting && Directory.Exists(workingDir))
                {
                    Directory.Delete(workingDir, true);
                }
                string databasePath = Path.Combine(workingDir, DatabaseName);
                if (!Directory.Exists(workingDir))
                {
                    Directory.CreateDirectory(workingDir);
                }

                _databaseDir   = workingDir;
                _isRegistering = true;

                this.CreateDatabase(databasePath, clearExisting);

                _databaseInstance = new Instance("opendatabase");

                // Creating an Instance object doesn't call JetInit. 
                // This is done to allow some parameters to be set
                // before the instance is initialized.

                // Circular logging is very useful; it automatically deletes
                // logfiles that are no longer needed. Most applications
                // should use it.
                _databaseInstance.Parameters.CircularLog = true;
                _databaseInstance.Parameters.BaseName = "snp";
                _databaseInstance.Parameters.LogFileDirectory = _databaseDir;
                _databaseInstance.Parameters.SystemDirectory = _databaseDir;
                _databaseInstance.Parameters.TempDirectory = _databaseDir;
                _databaseInstance.Parameters.AlternateDatabaseRecoveryDirectory = _databaseDir;

                // Initialize the instance. This creates the logs and temporary database.
                // If logs are present in the log directory then recovery will run
                // automatically.
                _databaseInstance.Init();

                _databaseSession = new Session(_databaseInstance);

                JET_DBID dbid;

                // The database only has to be attached once per instance, but each
                // session has to open the database. Redundant JetAttachDatabase calls
                // are safe to make though.
                // Here we use the fact that Instance, Session, and Table objects all have
                // implicit conversions to the underlying JET_* types. This allows the
                // disposable wrappers to be used with any APIs that expect the JET_*
                // structures.
                Api.JetAttachDatabase(_databaseSession, databasePath, 
                    AttachDatabaseGrbit.None);
                Api.JetOpenDatabase(_databaseSession, databasePath,
                    null, out dbid, OpenDatabaseGrbit.None);

                _databaseTable = new Table(_databaseSession, dbid, 
                    TableName, OpenTableGrbit.None);

                // Load the columnids from the table. This should be done each 
                // time the database is attached because an offline defrag 
                // (esentutl /d) can change the name => columnid mapping.
                IDictionary<string, JET_COLUMNID> columnids =
                    Api.GetColumnDictionary(_databaseSession, _databaseTable);
                _columnGroup = columnids["SnippetGroup"];
                _columnId    = columnids["SnippetID"];
                _columnLang  = columnids["SnippetLang"];
                _columnText  = columnids["SnippetSource"];

                // For the update of the database table...
                _databaseTransaction = new Transaction(_databaseSession);

                return true;
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);

                return false;
            }      
        }

        public override void FinishRegister()
        {
            _isRegistering = false;

            if (_databaseTransaction != null)
            {
                // Commit the transaction at the end of the using block!
                // If transaction.Commit isn't called then the transaction will 
                // automatically rollback when disposed (throwing away
                // the records that were just inserted).
                _databaseTransaction.Commit(CommitTransactionGrbit.None);
            }
       }

        public override void Register(Snippet snippet)
        {
            if (snippet == null)
            {
                throw new ArgumentNullException("snippet",
                    "The snippet cannot be null (or Nothing).");
            }

            this.Register(snippet.ExampleId, snippet.SnippetId,
                snippet.Language, snippet.Text);
        }

        public override void Register(SnippetInfo info, SnippetItem item)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info",
                    "The snippet information cannot be null (or Nothing).");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item",
                    "The snippet item cannot be null (or Nothing).");
            }

            this.Register(info.ExampleId, info.SnippetId,
                item.Language, item.Text);
        }

        public override void Register(string snippetGroup, string snippetId,
            string snippetLang, string snippetText)
        {
            // This will perform error checking...
            base.Register(snippetGroup, snippetId, snippetLang, snippetText);

            if (String.IsNullOrEmpty(snippetText))
            {
                return;
            }

            // Prepare an update, set some columns, and then save the update.
            // First create a disposable wrapper around JetPrepareUpdate and JetUpdate.
            using (var update = new Update(_databaseSession, _databaseTable,
                JET_prep.Insert))
            {
                Api.SetColumn(_databaseSession, _databaseTable,
                    _columnGroup, snippetGroup, Encoding.Unicode);
                Api.SetColumn(_databaseSession, _databaseTable,
                    _columnId, snippetId, Encoding.Unicode);
                Api.SetColumn(_databaseSession, _databaseTable,
                    _columnLang, snippetLang, Encoding.Unicode);
                Api.SetColumn(_databaseSession, _databaseTable,
                    _columnText, snippetText, Encoding.Unicode);

                // Save the update at the end of the using block!
                // If update.Save isn't called then the update will 
                // be canceled when disposed (and the record won't
                // be inserted).
                //
                // Inserting a record does not change the location of
                // the cursor (JET_TABLEID); it will have the same
                // location that it did before the insert.
                // To insert a record and then position the cursor
                // on the record, use Update.SaveAndGotoBookmark. That
                // call uses the bookmark returned from JetUpdate to
                // position the tableid on the new record.
                update.Save();

                _itemCount++;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a new database. Create the table, columns, and indexes.
        /// </summary>
        /// <param name="database">Name of the database to create.</param>
        private void CreateDatabase(string databasePath, bool clearExisting)
        {
            using (var instance = new Instance(Guid.NewGuid().ToString()))
            {
                if (String.IsNullOrEmpty(_databaseDir))
                {
                    _databaseDir = Path.GetDirectoryName(databasePath);
                }

                instance.Parameters.CircularLog = true;
                instance.Parameters.BaseName = "snp";
                instance.Parameters.LogFileDirectory = _databaseDir;
                instance.Parameters.SystemDirectory = _databaseDir;
                instance.Parameters.TempDirectory = _databaseDir;
                instance.Parameters.AlternateDatabaseRecoveryDirectory = _databaseDir;

                instance.Init();

                using (Session session = new Session(instance))
                {
                    JET_DBID dbid;
                    Api.JetCreateDatabase(session, databasePath, null, 
                        out dbid, clearExisting ? CreateDatabaseGrbit.OverwriteExisting
                        : CreateDatabaseGrbit.None);

                    using (Transaction transaction = new Transaction(session))
                    {
                        // A newly created table is opened exclusively. This is necessary to add
                        // a primary index to the table (a primary index can only be added if the table
                        // is empty and opened exclusively). Columns and indexes can be added to a 
                        // table which is opened normally.
                        // The other way to create a table is to use JetCreateTableColumnIndex to
                        // add all columns and indexes with one call.
                        JET_TABLEID tableid;
                        Api.JetCreateTable(session, dbid, TableName, 16, 100, out tableid);
                        CreateDatabaseColumns(session, tableid);
                        Api.JetCloseTable(session, tableid);

                        // Lazily commit the transaction. Normally committing a transaction forces the
                        // associated log records to be flushed to disk, so the commit has to wait for
                        // the I/O to complete. Using the LazyFlush option means that the log records
                        // are kept in memory and will be flushed later. This will preserve transaction
                        // atomicity (all operations in the transaction will either happen or be rolled
                        // back) but will not preserve durability (a crash after the commit call may
                        // result in the transaction updates being lost). Lazy transaction commits are
                        // considerably faster though, as they don't have to wait for an I/O.
                        transaction.Commit(CommitTransactionGrbit.LazyFlush);
                    }
                }
            }
        }

        /// <summary>
        /// Setup the meta-data for the given table.
        /// </summary>
        /// <param name="sessionId">The session to use.</param>
        /// <param name="tableId">
        /// The table to add the columns/indexes to. This table must be opened exclusively.
        /// </param>
        private void CreateDatabaseColumns(JET_SESID sessionId, 
            JET_TABLEID tableId)
        {
            using (Transaction transaction = new Transaction(sessionId))
            {
                JET_COLUMNID columnid;

                // SnippetGroup : text column
                var columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.Text,
                    cp     = JET_CP.Unicode
                };

                Api.JetAddColumn(sessionId, tableId, "SnippetGroup", columndef, 
                    null, 0, out columnid);

                // SnippetID : text column
                Api.JetAddColumn(sessionId, tableId, "SnippetID", columndef, 
                    null, 0, out columnid);

                // SnippetLang : text column
                Api.JetAddColumn(sessionId, tableId, "SnippetLang", columndef, 
                    null, 0, out columnid);

                // SnippetSource : Large Text
                columndef = new JET_COLUMNDEF
                {
                    coltyp = JET_coltyp.LongText,
                    cp     = JET_CP.Unicode
                };

                Api.JetAddColumn(sessionId, tableId, "SnippetSource", columndef, 
                    null, 0, out columnid);

                // Now add indexes. An index consists of several index segments (see
                // EsentVersion.Capabilities.ColumnsKeyMost to determine the maximum number of
                // segments). Each segment consists of a sort direction ('+' for ascending,
                // '-' for descending), a column name, and a '\0' separator. The index definition
                // must end with "\0\0". The count of characters should include all terminators.

                // An index on the snippet group. This index is not unique.
                //string indexDef = "+SnippetGroup\0\0";
                //Api.JetCreateIndex(sessionId, tableId, "group", CreateIndexGrbit.None, indexDef, indexDef.Length, 100);
                                    
                // An index on the snippet ID. This index is not unique.
                string indexDef = "+SnippetGroup\0+SnippetID\0\0";
                Api.JetCreateIndex(sessionId, tableId, "identifier", CreateIndexGrbit.None, indexDef, indexDef.Length, 100);

                transaction.Commit(CommitTransactionGrbit.LazyFlush);
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_databaseTable != null)
            {
                _databaseTable.Dispose();
                _databaseTable = null;
            }
            if (_databaseSession != null)
            {
                _databaseSession.Dispose();
                _databaseSession = null;
            }
            if (_databaseInstance != null)
            {
                _databaseInstance.Term();
                _databaseInstance.Dispose();
                _databaseInstance = null;
            }      
           
            try
            {
                if (!String.IsNullOrEmpty(_databaseDir) && 
                    Directory.Exists(_databaseDir))
                {
                    Directory.Delete(_databaseDir, true);
                }
            }
            catch
            {                 	
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
