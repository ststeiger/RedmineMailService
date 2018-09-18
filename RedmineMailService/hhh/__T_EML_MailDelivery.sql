
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[T_EML_Delivery]') AND type in (N'U')) 
BEGIN 
CREATE TABLE dbo.T_EML_Delivery 
( 
	 EML_UID uniqueidentifier NULL 
	,EML_Module national character varying(500) NULL 
	,EML_From character varying(255) NULL 
	,EML_ReplyTo character varying(255) NULL 
	,EML_To character varying(MAX) NULL 
	,EML_CC character varying(MAX) NULL 
	,EML_BCC character varying(MAX) NULL 
	,EML_Sent datetime NULL 
	,EML_Exception national character varying(MAX) NULL 
	,EML_SendDate datetime NULL 
	,EML_SendStatus bit NULL 
); 
END 


GO 


CREATE TABLE dbo.T_EML_Template  
( 
	 EML_Template_UID uniqueidentifier NOT NULL 
	,EML_Template_DE national character varying(MAX) NULL 
	,EML_Template_FR national character varying(MAX) NULL 
	,EML_Template_IT national character varying(MAX) NULL 
	,EML_Template_EN national character varying(MAX) NULL 
	,EML_Template_SQL national character varying(500) NULL 
); 


GO 


CREATE TABLE dbo.T_EML_ZO_Attachments 
(
	 ZO_EML_Attach_UID uniqueidentifier NOT NULL 
	,ZO_EML_Attach_EML_Template_UID uniqueidentifier NOT NULL 
	,ZO_EML_Attach_File national character varying(500) NULL 
	,ZO_EML_Attach_Mime_UID uniqueidentifier NOT NULL 
); 


GO 

