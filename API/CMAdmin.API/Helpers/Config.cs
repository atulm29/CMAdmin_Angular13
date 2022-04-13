using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Helpers
{
    public static class Config
    {
        private static string ReadConfigKey(string Key)
        {
            if (SettingsConfigHelper.AppSetting(Key) == null)
                throw new Exception("Key '" + Key + "' not found in configuration");

            if (string.IsNullOrWhiteSpace(SettingsConfigHelper.AppSetting(Key)))
                throw new Exception("Key '" + Key + "' not configured, please check configuration. ");

            return SettingsConfigHelper.AppSetting(Key);
        }

        private static string ReadConfigKey(string Key, string DefaultValue)
        {
            if (string.IsNullOrWhiteSpace(SettingsConfigHelper.AppSetting(Key)))
                return DefaultValue;

            return SettingsConfigHelper.AppSetting(Key);
        }

        public static string MCQBConnectionString
        {
            get
            {
                return SettingsConfigHelper.AppSetting("MCQDB");                   
            }
        }

        public static string MangentoWSURL
        {
            get { return ReadConfigKey("MangentoWSURL"); }
        }

        public static string SQLQueryTimeout
        {
            get { return ReadConfigKey("SQLQueryTimeout", "30"); }
        }

        public static bool LogSQLMessages
        {
            get { return Convert.ToBoolean(ReadConfigKey("LogSQLMessages", "true")); }
        }
        public static string DefaultDateFormat
        {
            get { return ReadConfigKey("DefaultDateFormat", "MM/dd/yyyy"); }
        }
        public static string Logging
        {
            get { return ReadConfigKey("Logging", "1"); }
        }

        public static string LogFileName
        {
            get { return ReadConfigKey("LogFileName", "MCQAdmin"); }
        }

        public static string TempContentPath
        {
            get { return ReadConfigKey("tempContentPath", "C:\\Temp"); }
        }

        public static string ContentFolderPath
        {
            get { return ReadConfigKey("ContentFolderPath"); }
        }

        public static string ContentURL
        {
            get { return ReadConfigKey("ContentURL"); }
        }

        public static string WebAPIUri
        {
            get { return ReadConfigKey("WebAPIUri"); }
        }

        public static string TargetFolder
        {
            get { return ReadConfigKey("TargetFolder"); }
        }

        public static string CSVUploadPath
        {
            get { return ReadConfigKey("CSVUploadPath"); }
        }

        public static string FlipickDisplayEquationType
        {
            get { return ReadConfigKey("FlipickDisplayEquationType", "Latex"); }
        }

        public static string IsFlipickInstitute
        {
            get { return ReadConfigKey("IsFlipickInstitute"); }
        }

        public static string SendAssessmentNotification
        {
            get { return ReadConfigKey("SendAssessmentNotification", "0"); }
        }

        public static string TestModifiedMailTemplate
        {
            get { return ReadConfigKey("TestModifiedMailTemplate", ""); }
        }

        public static string TestCreationMailTemplate
        {
            get { return ReadConfigKey("TestCreationMailTemplate", ""); }
        }

        public static string IBPSPOTestId
        {
            get { return ReadConfigKey("IBPSPOTestId", "0"); }
        }

        public static string SendResultEmailNotification
        {
            get { return ReadConfigKey("SendResultEmailNotification", ""); }
        }

        public static string StudentLoginURL
        {
            get { return ReadConfigKey("StudentLoginURL", ""); }
        }

        public static string CrocodocURL
        {
            get { return ReadConfigKey("CrocodocURL", ""); }
        }

        public static string BaseURL
        {
            get { return ReadConfigKey("BaseURL", ""); }
        }

        public static string ResultEmailSubject
        {
            get { return ReadConfigKey("ResultEmailSubject", ""); }
        }

        public static string TrainingDashBoardUrl
        {
            get { return ReadConfigKey("TrainingDashBoardUrl", ""); }
        }

        public static string LRSUrl
        {
            get { return ReadConfigKey("LRSUrl", ""); }
        }

        public static string FromDays
        {
            get { return ReadConfigKey("FromDays", ""); }
        }

        public static string CrocoDocAPIKey
        {
            get { return ReadConfigKey("CrocoDocAPIKey", ""); }
        }

        public static string CorporateCourseLevel
        {
            get { return ReadConfigKey("CorporateCourseLevel", ""); }
        }

        public static string IsMagentoRequired
        {
            get { return ReadConfigKey("IsMagentoRequired", ""); }
        }

        public static string PublisherID
        {
            get { return ReadConfigKey("PublisherID", "5"); }
        }

        public static string InstructorCurriculumTabs
        {
            get { return ReadConfigKey("InstructorCurriculumTabs", ""); }
        }

        public static string ImportCurriculumButtons
        {
            get { return ReadConfigKey("ImportCurriculumButtons", ""); }
        }

        public static string CollegeId
        {
            get { return ReadConfigKey("CollegeId", ""); }
        }

        public static string InstructorHideOptions
        {
            get { return ReadConfigKey("InstructorHideOptions", ""); }
        }

        public static string LrsGradeBookUrl
        {
            get { return ReadConfigKey("LRSGradeBookURL", ""); }
        }
        public static string LandingPage
        {
            get { return ReadConfigKey("LandingPage", ""); }
        }

        public static string SendEmail
        {
            get { return ReadConfigKey("SendEmail", "0"); }
        }
        public static string LevelSettingsURL
        {
            get { return ReadConfigKey("LevelSettingsURL", ""); }
        }

        public static string LRSCourseCompletion
        {
            get { return ReadConfigKey("LRSCourseCompletion", ""); }
        }

        public static string OnlineLectureAPI
        {
            get { return ReadConfigKey("OnlineLectureAPI", ""); }
        }

        public static string SurveryFormsAPI
        {
            get { return ReadConfigKey("SurveryFormsAPI", ""); }
        }

        public static string OnlineLectureIs
        {
            get { return ReadConfigKey("OnlineLectureIs", ""); }
        }

        public static string BaseWebSite
        {
            get { return ReadConfigKey("BaseWebSite", ""); }
        }

        public static string BaseWebSitePath
        {
            get { return ReadConfigKey("BaseWebSitePath", ""); }
        }

        public static string StudentPortalPath
        {
            get { return ReadConfigKey("StudentPortalPath", ""); }
        }

        public static string DefaultWebSite
        {
            get { return ReadConfigKey("DefaultWebSite", ""); }
        }

        public static string InstructorDefaultWebSite
        {
            get { return ReadConfigKey("InstructorDefaultWebSite", ""); }
        }

        public static string ApplicationPoolName
        {
            get { return ReadConfigKey("ApplicationPoolName", ""); }
        }

        public static string InstructorBaseWebSite
        {
            get { return ReadConfigKey("InstructorBaseWebSite", ""); }
        }

        public static string CopyEpub
        {
            get { return ReadConfigKey("CopyEpub", "N"); }
        }

        public static string CopyTests
        {
            get { return ReadConfigKey("CopyTests", "N"); }
        }

        public static string InstructorPortalName
        {
            get { return ReadConfigKey("InstructorPortalName", ""); }
        }

        public static string ShowAdminCourseCount
        {
            get { return ReadConfigKey("ShowAdminCourseCount", ""); }
        }

        public static string SuperAdminLinks
        {
            get { return ReadConfigKey("SuperAdminLinks", ""); }
        }

        public static string CalendarTemplateURL
        {
            get { return ReadConfigKey("CalendarTemplateURL", ""); }
        }

        public static string EditorialBoardURL
        {
            get { return ReadConfigKey("EditorialBoardURL", ""); }
        }

        public static string CalendarBatchURL
        {
            get { return ReadConfigKey("CalendarBatchURL", ""); }
        }

        public static string ClientCollegeId
        {
            get { return ReadConfigKey("ClientCollegeId", ""); }
        }

        public static string CreateVirtualDirectory
        {
            get { return ReadConfigKey("CreateVirtualDirectory", "Y"); }
        }
        public static string CreateMagentoWebsite
        {
            get { return ReadConfigKey("CreateMagentoWebsite", "Y"); }
        }

        public static string MainStoreCode
        {
            get { return ReadConfigKey("MainStoreCode", ""); }
        }

        public static string MagentoAPIURL
        {
            get { return ReadConfigKey("MagentoAPIURL", ""); }
        }

        public static string MagentoCreateOrderURL
        {
            get { return ReadConfigKey("MagentoCreateOrderURL", ""); }
        }

        public static string CouponCode
        {
            get { return ReadConfigKey("CouponCode", ""); }
        }

        public static string AnalyticsLinks
        {
            get { return ReadConfigKey("AnalyticsLinks", ""); }
        }

        public static string AddMetaToCurriculum
        {
            get { return ReadConfigKey("AddMetaToCurriculum", ""); }
        }

        public static string ContentFolderPathDentist
        {
            get { return ReadConfigKey("ContentFolderPathDentist", ""); }
        }

        public static string ContentURLDentist
        {
            get
            {
                string contentUrl = ReadConfigKey("ContentURLDentist");
                if (!contentUrl.EndsWith("/"))
                    contentUrl = contentUrl + "/";
                return contentUrl;
            }
        }

        public static string SortTreeViewExamsBy
        {
            get { return ReadConfigKey("SortTreeViewExamsBy", "ExamId"); }
        }

        public static string ShowCurriculumOptions
        {
            get { return ReadConfigKey("ShowCurriculumOptions", ""); }
        }

        public static string Branding
        {
            get { return ReadConfigKey("Branding", "Default"); }
        }

        public static string ChatAPIURL
        {
            get { return ReadConfigKey("ChatAPIURL", ""); }
        }

        public static string SuperAdminDashboard
        {
            get { return ReadConfigKey("SuperAdminDashboard", ""); }
        }

        public static string LRSSchedulerPath
        {
            get { return ReadConfigKey("LRSSchedulerPath", ""); }
        }

        public static string LRSApi
        {
            get { return ReadConfigKey("LRSApi", ""); }
        }

        public static string ManageOnlineLectureURL
        {
            get { return ReadConfigKey("ManageOnlineLectureURL", ""); }
        }

        public static string HideReferenceCurriculumTabs
        {
            get { return ReadConfigKey("HideReferenceCurriculumTabs", ""); }
        }

        public static string ViewQuestionRedirectTo
        {
            get { return ReadConfigKey("ViewQuestionRedirectTo", "QuestionList"); }
        }

        public static string ScheduleOnlineLecture
        {
            get { return ReadConfigKey("ScheduleOnlineLecture", ""); }
        }

        public static string isExcludeVisible
        {
            get { return ReadConfigKey("isExcludeVisible", "0"); }
        }

        public static string SemesterAlias
        {
            get { return ReadConfigKey("SemesterAlias", "Semester"); }
        }

        public static string ShowPrerequisite
        {
            get { return ReadConfigKey("ShowPrerequisite", "Y"); }
        }

        public static string DefaultCourseType
        {
            get { return ReadConfigKey("DefaultCourseType", ""); }
        }

        public static string AppendArticulateURL
        {
            get { return ReadConfigKey("AppendArticulateURL", ""); }
        }

        public static string PDFTOSVGURL
        {
            get { return ReadConfigKey("PDFTOSVGURL", ""); }
        }

        public static string DefaultCollegeID
        {
            get { return ReadConfigKey("DefaultCollegeID", ""); }
        }

        public static string ShowLevels
        {
            get { return ReadConfigKey("ShowLevels", "Y"); }
        }

        public static string ShowSkills
        {
            get { return ReadConfigKey("ShowSkills", "Y"); }
        }

        public static string ShowCourseType
        {
            get { return ReadConfigKey("ShowCourseType", "Y"); }
        }
        public static string ShowTopicCompletion
        {
            get { return ReadConfigKey("ShowTopicCompletion", "Y"); }
        }
        public static string ShowLRS
        {
            get { return ReadConfigKey("ShowLRS", "Y"); }
        }
        public static string ShowSubscriptionMonths
        {
            get { return ReadConfigKey("ShowSubscriptionMonths", "Y"); }
        }
        public static string ShowTotalSlides
        {
            get { return ReadConfigKey("ShowTotalSlides", "Y"); }
        }
        public static string ShowCreateFromMaster
        {
            get { return ReadConfigKey("ShowCreateFromMaster", "Y"); }
        }
        public static string ShowCopyContent
        {
            get { return ReadConfigKey("ShowCopyContent", "Y"); }
        }
        public static string ShowMockTestFormats
        {
            get { return ReadConfigKey("ShowMockTestFormats", "Y"); }
        }
        public static string SortTreeViewSemesterBy
        {
            get { return ReadConfigKey("SortTreeViewSemesterBy", "SemesterName"); }
        }

        public static string SearchAnimationsURL
        {
            get { return ReadConfigKey("SearchAnimationsURL", ""); }
        }

        public static string MyPresentationsURL
        {
            get { return ReadConfigKey("MyPresentationsURL", ""); }
        }

        public static string ShowBatchOnly
        {
            get { return ReadConfigKey("ShowBatchOnly", ""); }
        }
        public static string StoreCollegeId
        {
            get { return ReadConfigKey("StoreCollegeId", ""); }
        }

        public static string OfficeToHtml
        {
            get { return ReadConfigKey("OfficeToHtml", ""); }
        }
        public static bool IsQuestionToBeEncrypted
        {
            get { return ReadConfigKey("IsQuestionToBeEncrypted", "N").ToUpper() == "Y" ? true : false; }
        }

        public static string QBankWorkFlowURL
        {
            get { return ReadConfigKey("QBankWorkFlowURL", ""); }
        }

        public static string IsAdditionalTestSetting
        {
            get { return ReadConfigKey("IsAdditionalTestSetting", "N"); }
        }

        public static string AssetAuthoringWorkflowURL
        {
            get { return ReadConfigKey("AssetAuthoringWorkflowURL", ""); }
        }

        public static string HtmlEditorURL
        {
            get { return ReadConfigKey("HtmlEditorURL", ""); }
        }
        public static string LaunchMCQPreviewURL
        {
            get { return ReadConfigKey("LaunchMCQPreviewURL", ""); }
        }

        public static string LTIDispatchUrl
        {
            get { return ReadConfigKey("LTIDispatchUrl", ""); }
        }

        public static string IsRemedial
        {
            get { return ReadConfigKey("IsRemedial", ""); }
        }

        public static string EnableAssessmentBank
        {
            get { return ReadConfigKey("EnableAssessmentBank", "N"); }
        }
        public static string EnableExamPattern
        {
            get { return ReadConfigKey("EnableExamPattern", "N"); }
        }

        public static string CopyCourseFromOrgId
        {
            get { return ReadConfigKey("CopyCourseFromOrgId", "0"); }
        }

        public static string StudentPortalURL
        {
            get { return ReadConfigKey("StudentPortalURL", "0"); }
        }

        public static string EnableCourseVesrion
        {
            get { return ReadConfigKey("EnableCourseVesrion", "N"); }
        }

        public static string ScoreCardURL
        {
            get { return ReadConfigKey("ScoreCardURL", ""); }
        }
        public static string AllowMultipleAccounts
        {
            get { return ReadConfigKey("AllowMultipleAccounts", ""); }
        }

        public static string PDFLibrary
        {
            get { return ReadConfigKey("PDFLibrary", ""); }
        }

        public static string ContentPDFURL
        {
            get { return ReadConfigKey("ContentPDFURL", ""); }
        }

        public static string EnableSelfAssessment
        {
            get { return ReadConfigKey("EnableSelfAssessment", "N"); }
        }

        public static string EnablePDFConvertToHTML
        {
            get { return ReadConfigKey("EnablePDFConvertToHTML", "N"); }
        }
        //public static string DefaultDateFormat
        //{
        //    get { return ReadConfigKey("DefaultDateFormat", "dd/MM/yyyy"); }
        //}

        public static string SchedulerEXEPath
        {
            get { return ReadConfigKey("SchedulerEXEPath", ""); }
        }
    }
}
