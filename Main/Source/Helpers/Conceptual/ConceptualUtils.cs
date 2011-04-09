using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandcastle.Conceptual
{
    public static class ConceptualUtils
    {
        #region Private Static Fields

        private static Regex validId = new Regex(
            "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", 
            RegexOptions.Compiled);

        #endregion

        #region Public Static Methods

        public static bool IsValidId(string testId)
        {
            if (String.IsNullOrEmpty(testId))
            {
                return false;
            }
            if (testId.Length != 36)
            {
                return false;
            }

            return validId.IsMatch(testId);
        }

        public static bool IsValidTopicTypeId(string topicTypeId)
        {
            if (String.IsNullOrEmpty(topicTypeId))
            {
                return false;
            }
            if (topicTypeId.Length != 36)
            {
                return false;
            }

            switch (topicTypeId.ToUpper())
            {
                // Concepts
                case "1FE70836-AA7D-4515-B54B-E10C4B516E50": // Conceptual
                    return true;
                case "68F07632-C4C5-4645-8DFA-AC87DCB4BD54": // SDK Technology Overview Architecture
                    return true;
                case "4BBAAF90-0E5F-4C86-9D31-A5CAEE35A416": // SDK Technology Overview Code Directory
                    return true;
                case "356C57C4-384D-4AF2-A637-FDD6F088A033": // SDK Technology Overview Scenarios
                    return true;
                case "19F1BB0E-F32A-4D5F-80A9-211D92A8A715": // SDK Technology Overview Technology Summary
                    return true;
                // Tasks
                case "DAC3A6A0-C863-4E5B-8F65-79EFC6A4BA09": // HowTo
                    return true;
                case "4779DD54-5D0C-4CC3-9DB3-BF1C90B721B3": // Walkthrough
                    return true;
                case "069EFD88-412D-4E2F-8848-2D5C3AD56BDE": // Sample
                    return true;
                case "38C8E0D1-D601-4DBA-AE1B-5BEC16CD9B01": // Troubleshooting
                    return true;
                // Reference
                case "F9205737-4DEC-4A58-AA69-0E621B1236BD": // Reference Without Syntax
                    return true;
                case "95DADC4C-A2A6-447A-AA36-B6BE3A4F8DEC": // Reference With Syntax
                    return true;
                case "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F": // XML Reference
                    return true;
                case "A635375F-98C2-4241-94E7-E427B47C20B6": // Error Message
                    return true;
                case "B8ED9F21-39A4-4967-928D-160CD2ED9DCE": // UI Reference
                    return true;
                // Other Resources
                case "B137C930-7BF7-48A2-A329-3ADCAEF8868E": // Orientation
                    return true;
                case "56DB00EC-28BA-4C0D-8694-28E8B244E236": // White Paper
                    return true;
                case "4A273212-0AC8-4D72-8349-EC11CD2FF8CD": // Code Entity
                    return true;
                case "A689E19C-2687-4881-8CE1-652FF60CF46C": // Glossary
                    return true;
                case "CDB8C120-888F-447B-8AF8-F9540562E7CA": // SDK Technology Overview Orientation
                    return true;
            }

            return false;
        }

        public static bool IsValidDocumentTag(string documentTag)
        {
            if (String.IsNullOrEmpty(documentTag))
            {
                return false;
            }

            switch (documentTag)
            {
                // Concepts
                case "developerConceptualDocument": // Conceptual
                    return true;
                case "developerSDKTechnologyOverviewArchitectureDocument": // SDK Technology Overview Architecture
                    return true;
                case "developerSDKTechnologyOverviewCodeDirectoryDocument": // SDK Technology Overview Code Directory
                    return true;
                case "developerSDKTechnologyOverviewScenariosDocument": // SDK Technology Overview Scenarios
                    return true;
                case "developerSDKTechnologyOverviewTechnologySummaryDocument": // SDK Technology Overview Technology Summary
                    return true;
                // Tasks
                case "developerHowToDocument": // HowTo
                    return true;
                case "developerWalkthroughDocument": // Walkthrough
                    return true;
                case "developerSampleDocument": // Sample
                    return true;
                case "developerTroubleshootingDocument": // Troubleshooting
                    return true;
                // Reference
                case "developerReferenceWithoutSyntaxDocument": // Reference Without Syntax
                    return true;
                case "developerReferenceWithSyntaxDocument": // Reference With Syntax
                    return true;
                case "developerXmlReference": // XML Reference
                    return true;
                case "developerErrorMessageDocument": // Error Message
                    return true;
                case "developerUIReferenceDocument": // UI Reference
                    return true;
                // Other Resources
                case "developerOrientationDocument": // Orientation
                    return true;
                case "developerWhitePaperDocument": // White Paper
                    return true;
                case "codeEntityDocument": // Code Entity
                    return true;
                case "developerGlossaryDocument": // Glossary
                    return true;
                case "developerSDKTechnologyOverviewOrientationDocument": // SDK Technology Overview Orientation
                    return true;
            }

            return false;
        }

        public static string ToTopicTypeId(ConceptualTopicType topicType)
        {
            switch (topicType)
            {
                // Concepts
                case ConceptualTopicType.Conceptual: // Conceptual
                    return "1FE70836-AA7D-4515-B54B-E10C4B516E50";
                case ConceptualTopicType.SDKTechOverviewArchitecture: // SDK Technology Overview Architecture
                    return "68F07632-C4C5-4645-8DFA-AC87DCB4BD54";
                case ConceptualTopicType.SDKTechOverviewCodeDirectory: // SDK Technology Overview Code Directory
                    return "4BBAAF90-0E5F-4C86-9D31-A5CAEE35A416";
                case ConceptualTopicType.SDKTechOverviewScenarios: // SDK Technology Overview Scenarios
                    return "356C57C4-384D-4AF2-A637-FDD6F088A033";
                case ConceptualTopicType.SDKTechOverviewTechSummary: // SDK Technology Overview Technology Summary
                    return "19F1BB0E-F32A-4D5F-80A9-211D92A8A715";
                // Tasks
                case ConceptualTopicType.HowTo: // HowTo
                    return "DAC3A6A0-C863-4E5B-8F65-79EFC6A4BA09";
                case ConceptualTopicType.Walkthrough: // Walkthrough
                    return "4779DD54-5D0C-4CC3-9DB3-BF1C90B721B3";
                case ConceptualTopicType.Sample: // Sample
                    return "069EFD88-412D-4E2F-8848-2D5C3AD56BDE";
                case ConceptualTopicType.Troubleshooting: // Troubleshooting
                    return "38C8E0D1-D601-4DBA-AE1B-5BEC16CD9B01";
                // Reference
                case ConceptualTopicType.ReferenceWithoutSyntax: // Reference Without Syntax
                    return "F9205737-4DEC-4A58-AA69-0E621B1236BD";
                case ConceptualTopicType.ReferenceWithSyntax: // Reference With Syntax
                    return "95DADC4C-A2A6-447A-AA36-B6BE3A4F8DEC";
                case ConceptualTopicType.XmlReference: // XML Reference
                    return "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F";
                case ConceptualTopicType.ErrorMessage: // Error Message
                    return "A635375F-98C2-4241-94E7-E427B47C20B6";
                case ConceptualTopicType.UIReference: // UI Reference
                    return "B8ED9F21-39A4-4967-928D-160CD2ED9DCE";
                // Other Resources
                case ConceptualTopicType.Orientation: // Orientation
                    return "B137C930-7BF7-48A2-A329-3ADCAEF8868E";
                case ConceptualTopicType.WhitePaper: // White Paper
                    return "56DB00EC-28BA-4C0D-8694-28E8B244E236";
                case ConceptualTopicType.CodeEntity: // Code Entity
                    return "4A273212-0AC8-4D72-8349-EC11CD2FF8CD";
                case ConceptualTopicType.Glossary: // Glossary
                    return "A689E19C-2687-4881-8CE1-652FF60CF46C";
                case ConceptualTopicType.SDKTechOverviewOrientation: // SDK Technology Overview Orientation
                    return "CDB8C120-888F-447B-8AF8-F9540562E7CA";
            }

            return String.Empty;
        }

        public static string ToTopicTypeId(string documentTag)
        {
            if (String.IsNullOrEmpty(documentTag))
            {
                return String.Empty;
            }

            switch (documentTag)
            {
                // Concepts
                case "developerConceptualDocument": // Conceptual
                    return "1FE70836-AA7D-4515-B54B-E10C4B516E50";
                case "developerSDKTechnologyOverviewArchitectureDocument": // SDK Technology Overview Architecture
                    return "68F07632-C4C5-4645-8DFA-AC87DCB4BD54";
                case "developerSDKTechnologyOverviewCodeDirectoryDocument": // SDK Technology Overview Code Directory
                    return "4BBAAF90-0E5F-4C86-9D31-A5CAEE35A416";
                case "developerSDKTechnologyOverviewScenariosDocument": // SDK Technology Overview Scenarios
                    return "356C57C4-384D-4AF2-A637-FDD6F088A033";
                case "developerSDKTechnologyOverviewTechnologySummaryDocument": // SDK Technology Overview Technology Summary
                    return "19F1BB0E-F32A-4D5F-80A9-211D92A8A715";
                // Tasks
                case "developerHowToDocument": // HowTo
                    return "DAC3A6A0-C863-4E5B-8F65-79EFC6A4BA09";
                case "developerWalkthroughDocument": // Walkthrough
                    return "4779DD54-5D0C-4CC3-9DB3-BF1C90B721B3";
                case "developerSampleDocument": // Sample
                    return "069EFD88-412D-4E2F-8848-2D5C3AD56BDE";
                case "developerTroubleshootingDocument": // Troubleshooting
                    return "38C8E0D1-D601-4DBA-AE1B-5BEC16CD9B01";
                // Reference
                case "developerReferenceWithoutSyntaxDocument": // Reference Without Syntax
                    return "F9205737-4DEC-4A58-AA69-0E621B1236BD";
                case "developerReferenceWithSyntaxDocument": // Reference With Syntax
                    return "95DADC4C-A2A6-447A-AA36-B6BE3A4F8DEC";
                case "developerXmlReference": // XML Reference
                    return "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F";
                case "developerErrorMessageDocument": // Error Message
                    return "A635375F-98C2-4241-94E7-E427B47C20B6";
                case "developerUIReferenceDocument": // UI Reference
                    return "B8ED9F21-39A4-4967-928D-160CD2ED9DCE";
                // Other Resources
                case "developerOrientationDocument": // Orientation
                    return "B137C930-7BF7-48A2-A329-3ADCAEF8868E";
                case "developerWhitePaperDocument": // White Paper
                    return "56DB00EC-28BA-4C0D-8694-28E8B244E236";
                case "codeEntityDocument": // Code Entity
                    return "4A273212-0AC8-4D72-8349-EC11CD2FF8CD";
                case "developerGlossaryDocument": // Glossary
                    return "A689E19C-2687-4881-8CE1-652FF60CF46C";
                case "developerSDKTechnologyOverviewOrientationDocument": // SDK Technology Overview Orientation
                    return "CDB8C120-888F-447B-8AF8-F9540562E7CA";
            }

            return String.Empty;
        }

        public static ConceptualTopicType FromTopicTypeId(string topicTypeId)
        {
            if (String.IsNullOrEmpty(topicTypeId))
            {
                return ConceptualTopicType.None;
            }
            if (topicTypeId.Length != 36)
            {
                return ConceptualTopicType.None;
            }

            switch (topicTypeId.ToUpper())
            {
                // Concepts
                case "1FE70836-AA7D-4515-B54B-E10C4B516E50": // Conceptual
                    return ConceptualTopicType.Conceptual;
                case "68F07632-C4C5-4645-8DFA-AC87DCB4BD54": // SDK Technology Overview Architecture
                    return ConceptualTopicType.SDKTechOverviewArchitecture;
                case "4BBAAF90-0E5F-4C86-9D31-A5CAEE35A416": // SDK Technology Overview Code Directory
                    return ConceptualTopicType.SDKTechOverviewCodeDirectory;
                case "356C57C4-384D-4AF2-A637-FDD6F088A033": // SDK Technology Overview Scenarios
                    return ConceptualTopicType.SDKTechOverviewScenarios;
                case "19F1BB0E-F32A-4D5F-80A9-211D92A8A715": // SDK Technology Overview Technology Summary
                    return ConceptualTopicType.SDKTechOverviewTechSummary;
                // Tasks
                case "DAC3A6A0-C863-4E5B-8F65-79EFC6A4BA09": // HowTo
                    return ConceptualTopicType.HowTo;
                case "4779DD54-5D0C-4CC3-9DB3-BF1C90B721B3": // Walkthrough
                    return ConceptualTopicType.Walkthrough;
                case "069EFD88-412D-4E2F-8848-2D5C3AD56BDE": // Sample
                    return ConceptualTopicType.Sample;
                case "38C8E0D1-D601-4DBA-AE1B-5BEC16CD9B01": // Troubleshooting
                    return ConceptualTopicType.Troubleshooting;
                // Reference
                case "F9205737-4DEC-4A58-AA69-0E621B1236BD": // Reference Without Syntax
                    return ConceptualTopicType.ReferenceWithoutSyntax;
                case "95DADC4C-A2A6-447A-AA36-B6BE3A4F8DEC": // Reference With Syntax
                    return ConceptualTopicType.ReferenceWithSyntax;
                case "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F": // XML Reference
                    return ConceptualTopicType.XmlReference;
                case "A635375F-98C2-4241-94E7-E427B47C20B6": // Error Message
                    return ConceptualTopicType.ErrorMessage;
                case "B8ED9F21-39A4-4967-928D-160CD2ED9DCE": // UI Reference
                    return ConceptualTopicType.UIReference;
                // Other Resources
                case "B137C930-7BF7-48A2-A329-3ADCAEF8868E": // Orientation
                    return ConceptualTopicType.Orientation;
                case "56DB00EC-28BA-4C0D-8694-28E8B244E236": // White Paper
                    return ConceptualTopicType.WhitePaper;
                case "4A273212-0AC8-4D72-8349-EC11CD2FF8CD": // Code Entity
                    return ConceptualTopicType.CodeEntity;
                case "A689E19C-2687-4881-8CE1-652FF60CF46C": // Glossary
                    return ConceptualTopicType.Glossary;
                case "CDB8C120-888F-447B-8AF8-F9540562E7CA": // SDK Technology Overview Orientation
                    return ConceptualTopicType.SDKTechOverviewOrientation;
            }

            return ConceptualTopicType.None;
        }

        public static ConceptualTopicType FromDocumentTag(string documentTag)
        {
            if (String.IsNullOrEmpty(documentTag))
            {
                return ConceptualTopicType.None;
            }

            switch (documentTag)
            {
                // Concepts
                case "developerConceptualDocument": // Conceptual
                    return ConceptualTopicType.Conceptual;
                case "developerSDKTechnologyOverviewArchitectureDocument": // SDK Technology Overview Architecture
                    return ConceptualTopicType.SDKTechOverviewArchitecture;
                case "developerSDKTechnologyOverviewCodeDirectoryDocument": // SDK Technology Overview Code Directory
                    return ConceptualTopicType.SDKTechOverviewCodeDirectory;
                case "developerSDKTechnologyOverviewScenariosDocument": // SDK Technology Overview Scenarios
                    return ConceptualTopicType.SDKTechOverviewScenarios;
                case "developerSDKTechnologyOverviewTechnologySummaryDocument": // SDK Technology Overview Technology Summary
                    return ConceptualTopicType.SDKTechOverviewTechSummary;
                // Tasks
                case "developerHowToDocument": // HowTo
                    return ConceptualTopicType.HowTo;
                case "developerWalkthroughDocument": // Walkthrough
                    return ConceptualTopicType.Walkthrough;
                case "developerSampleDocument": // Sample
                    return ConceptualTopicType.Sample;
                case "developerTroubleshootingDocument": // Troubleshooting
                    return ConceptualTopicType.Troubleshooting;
                // Reference
                case "developerReferenceWithoutSyntaxDocument": // Reference Without Syntax
                    return ConceptualTopicType.ReferenceWithoutSyntax;
                case "developerReferenceWithSyntaxDocument": // Reference With Syntax
                    return ConceptualTopicType.ReferenceWithSyntax;
                case "developerXmlReference": // XML Reference
                    return ConceptualTopicType.XmlReference;
                case "developerErrorMessageDocument": // Error Message
                    return ConceptualTopicType.ErrorMessage;
                case "developerUIReferenceDocument": // UI Reference
                    return ConceptualTopicType.UIReference;
                // Other Resources
                case "developerOrientationDocument": // Orientation
                    return ConceptualTopicType.Orientation;
                case "developerWhitePaperDocument": // White Paper
                    return ConceptualTopicType.WhitePaper;
                case "codeEntityDocument": // Code Entity
                    return ConceptualTopicType.CodeEntity;
                case "developerGlossaryDocument": // Glossary
                    return ConceptualTopicType.Glossary;
                case "developerSDKTechnologyOverviewOrientationDocument": // SDK Technology Overview Orientation
                    return ConceptualTopicType.SDKTechOverviewOrientation;
            }

            return ConceptualTopicType.None;
        }

        #endregion
    }
}
