using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Data;
using System.Data.Common;
using System.Data.SQLite;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetSqliteProvider : SnippetProvider
    {
        #region Private Constant Fields

        private const string CodeSnippetDB = "CodeSnippets.db3";

        #endregion

        #region Private Fields

        private int           _itemCount;
        private bool          _isRegistering;

        private string        _dbSource;

        private DbCommand     _dbInsert;
        private DbCommand     _dbCommand;
        private DbParameter   _dbSnippetGroup;
        private DbParameter   _dbSnippetID;
        private DbParameter   _dbSnippetLang;
        private DbParameter   _dbSnippetSource;
        private DbConnection  _dbConnection;

        private StringBuilder _sqlBuilder;

        #endregion

        #region Constructors and Destructor

        public SnippetSqliteProvider(Type componentType, MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
            _sqlBuilder = new StringBuilder();
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
                return SnippetStorage.Sqlite;
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
                if (_itemCount <= 0 || _dbConnection == null)
                {
                    return null;
                }

                // Build the SQL query from the specified snippet information...
                _sqlBuilder.Length = 0;
                _sqlBuilder.Append("SELECT SnippetLang, SnippetSource FROM Snippets ");
                _sqlBuilder.AppendFormat(
                    "WHERE ((SnippetGroup = '{0}') AND (SnippetID = '{1}'))",
                    info.ExampleId, info.SnippetId);

                DbDataReader dbReader = null; // keep the compiler happy...
                try
                {
                    _dbConnection.Open();

                    _dbCommand.Connection = _dbConnection;
                    _dbCommand.CommandText = _sqlBuilder.ToString();

                    dbReader = _dbCommand.ExecuteReader();

                    List<SnippetItem> listInfo = null;
                    if (dbReader != null)
                    {
                        listInfo = new List<SnippetItem>();
                        while (dbReader.Read())
                        {
                            listInfo.Add(new SnippetItem(dbReader.GetString(0),
                                dbReader.GetString(1)));
                        }
                    }

                    dbReader.Close();
                    dbReader = null;

                    _dbConnection.Close();

                    return listInfo;
                }
                catch (SQLiteException ex)
                {
                    if (dbReader != null)
                    {
                        dbReader.Close();
                        dbReader = null;
                    }

                    if (_dbConnection.State != ConnectionState.Closed)
                    {
                        _dbConnection.Close();
                    }

                    this.WriteMessage(MessageLevel.Info, _sqlBuilder.ToString());
                    this.WriteMessage(MessageLevel.Error, ex);

                    return null;
                }
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
                _isRegistering = true;

                //TODO - PAUL: Use transactions to speed?
                // Consult the Sqlite experts to find out if significant memory
                // is consumed during the Sqlite Transaction state.
                string dbSource = CodeSnippetDB;

                if (clearExisting)
                {
                    File.Delete(dbSource);
                }

                _dbSource = dbSource;

                _dbConnection = new SQLiteConnection();
                _dbConnection.ConnectionString = String.Format(
                    "Data Source={0};Pooling=False", dbSource);
                _dbConnection.Open();

                _dbCommand = _dbConnection.CreateCommand();
                string dbTable = @"CREATE TABLE Snippets 
                    (ID integer PRIMARY KEY AUTOINCREMENT, 
                     SnippetGroup NVARCHAR(125), SnippetID NVARCHAR(32), 
                     SnippetLang NVARCHAR(32), SnippetSource NVARCHAR(4000))";
                _dbCommand.CommandText = dbTable;
                _dbCommand.ExecuteNonQuery();

                string insertStatement = @"INSERT INTO Snippets(SnippetGroup, 
                SnippetID, SnippetLang, SnippetSource) VALUES(?, ?, ?, ?);
                SELECT last_insert_rowid()";
                _dbInsert = _dbConnection.CreateCommand();
                _dbInsert.CommandText = insertStatement;

                _dbSnippetGroup  = _dbInsert.CreateParameter();
                _dbSnippetID     = _dbInsert.CreateParameter();
                _dbSnippetLang   = _dbInsert.CreateParameter();
                _dbSnippetSource = _dbInsert.CreateParameter();

                _dbInsert.Parameters.Add(_dbSnippetGroup);
                _dbInsert.Parameters.Add(_dbSnippetID);
                _dbInsert.Parameters.Add(_dbSnippetLang);
                _dbInsert.Parameters.Add(_dbSnippetSource);

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
            if (_dbInsert != null)
            {
                _dbInsert.Dispose();
                _dbInsert = null;
            }

            _dbSnippetGroup  = null;
            _dbSnippetID     = null;
            _dbSnippetLang   = null;
            _dbSnippetSource = null;

            // We close the connection for now, and wait for query.
            if (_dbConnection != null && 
                _dbConnection.State != ConnectionState.Closed)
            {
                _dbConnection.Close();
            }
        }

        public override void Register(Snippet snippet)
        {
            if (snippet == null)
            {
                throw new ArgumentNullException("snippet",
                    "The snippet cannot be null (or Nothing).");
            }

            this.Register(new SnippetInfo(snippet.ExampleId, snippet.SnippetId),
                new SnippetItem(snippet.Language, snippet.Text));
        }

        public override void Register(string snippetGroup, string snippetId,
            string snippetLang, string snippetText)
        {
            base.Register(snippetGroup, snippetId, snippetLang, snippetText);

            if (String.IsNullOrEmpty(snippetText))
            {
                return;
            }

            if (_dbInsert == null)
            {
                return;
            }

            _dbSnippetGroup.Value  = snippetGroup;
            _dbSnippetID.Value     = snippetId;
            _dbSnippetLang.Value   = snippetLang;
            _dbSnippetSource.Value = snippetText;

            int lastIndex = Convert.ToInt32(_dbInsert.ExecuteScalar());
            if (lastIndex > 0)
            {
                _itemCount = lastIndex;
            }
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
            if (_dbInsert == null)
            {
                return;
            }

            _dbSnippetGroup.Value  = info.ExampleId;
            _dbSnippetID.Value     = info.SnippetId;
            _dbSnippetLang.Value   = item.Language;
            _dbSnippetSource.Value = item.Text;

            int lastIndex = Convert.ToInt32(_dbInsert.ExecuteScalar());
            if (lastIndex > 0)
            {
                _itemCount = lastIndex;
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_dbInsert != null)
            {
                _dbInsert.Dispose();
                _dbInsert = null;
            }

            if (_dbConnection != null)
            {
                _dbConnection.Dispose();
                _dbConnection = null;
            }

            if (!String.IsNullOrEmpty(_dbSource) && File.Exists(_dbSource))
            {
                File.Delete(_dbSource);
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
