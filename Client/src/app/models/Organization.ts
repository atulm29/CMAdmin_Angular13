export class OrganizationTableData {
    rowNum?: string;
    groupName?: string;
    totalInstitute?: string;
    instructorCount?: string;
    paidStudentCount?: string;
    trialStudentCount?: string;
  }

  export class OrganizationTableHeader{
    srNo?: string;
    groupConfigLabel?: string;
    instituteLabelConfig?: string;
    instructorLabelConfig?: string;
    subScribeStudentLabelConfig?: string;
    trialStudentLabelConfig?: string;
  }

  export class OrganizationDropDownData {
    groupId?: number;
    groupName?: string;
    collegeId?: number;
    showNewUserTab?: string;
    showAssessmentsTab?: string;
    masterCourseId?: number;
    assessmentTabName?: string;
    organizationType?: string;
    organizationId?: number;
  }