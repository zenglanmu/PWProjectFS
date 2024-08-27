using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PWProjectFS.PWApiWrapper
{
	public class dmscli
	{
		public enum IType
		{
			Unknown = 0,
			History = 11,
			Set = 12,
			Redline = 13,
			Modeler_BRP = 14,
			Abstract = 15
		}

		public enum SetType
		{
			Unknown = 0,
			Flat = 2,
			Logical = 3
		}

		public enum SetRelationType
		{
			Sibling = 2,
			Redline,
			Reference
		}

		public enum TypeMask
		{
			Flat = 65536,
			Logical = 131072,
			Redline = 524288,
			Ref = 1048576,
			All = 2031616
		}

		public abstract class TransferType
		{
			public static readonly string Copy = "C";

			public static readonly string Checkout = "CO";

			public static readonly string Unknown = "ND";
		}

		public enum DataSourceProperty
		{
			Type = 1,
			NativeType,
			Name,
			InternalName,
			SrvAddress,
			IsPublishedByConnSrv,
			FullName
		}

		public enum SetProperty
		{
			ID = 1,
			MemberId,
			Type,
			ParentProjectId,
			ParentItemId,
			ChildProjectId,
			ChildItemId,
			Relation,
			Transfer,
			SetProjectID,
			SetItemId,
			SDocGuid,
			PDocGuid,
			CDocGuid
		}

		public enum DocumentProperty
		{
			ID = 1,
			VersionNumber = 2,
			ProposalNumber = 3,
			CreatorID = 4,
			UpdaterID = 5,
			UserID = 6,
			Size = 7,
			FileType = 8,
			ItemType = 9,
			StorageID = 10,
			SetID = 11,
			SetType = 12,
			WorkFlowID = 13,
			StateID = 14,
			ApplicationID = 15,
			DepartmentID = 16,
			OriginalNumber = 18,
			IsOutToMe = 19,
			Name = 20,
			FileName = 21,
			Desc = 22,
			Version = 23,
			CreateTime = 24,
			UpdateTime = 25,
			DMSStatus = 26,
			DMSDate = 27,
			Node = 28,
			ProjectID = 29,
			Access = 30,
			IsLogicalSetMaster = 31,
			IsRedlineMaster = 32,
			IsRefMaster = 33,
			HasFinalStatus = 35,
			Manager = 36,
			FileUpdater = 37,
			LastRtLocker = 38,
			ItemFlags = 39,
			FileUpdateTime = 40,
			LastRtLockTime = 41,
			MgrType = 44,
			IsUrl = 45,
			UrlName = 46,
			DocGuid = 47,
			ProjGuid = 48,
			OrigGuid = 49,
			WSpaceProfID = 50,
			Is3DFile = 51,
			Is2DFile = 52,
			FileRevision = 53,
			Overlaps = 54,
			MimeType = 56
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct AADOCSELECT_ITEM
        {
			public uint ulFlags; // ULONG is typically 32-bit unsigned integer in C#
			public int lEnvironmentId; // LONG is typically a 32-bit signed integer in C#
			public int lProjectId;
			public int lDocumentId;
			public int lSetId;
			public int lSetType;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrFileName; // LPCWSTR is a pointer to a wide string (UTF-16)

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrName;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrDescription;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrVersion;

			public int lVersionSeq;
			public int lOriginal;
			public int lCreatorId;
			public int lUpdaterId;
			public int lLastUserId;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrStatus;

			public int lFileType;
			public int lItemType;
			public int lStorageId;
			public int lWorkflowId;
			public int lStateId;
			public int lApplicationId;
			public int lDepartmentId;
			public int lManagerId;
			public int lItemFlags;
			public int lManagerType;

			public IntPtr lpProject; // Assuming LPAADMSPROJITEM is a pointer type

			public int lFileUpdator;
		}

		public enum ProjectProperty
		{
			NONE = -1,
			ID = 1,
			VersionNo = 2,
			ManagerID = 3,
			StorageID = 4,
			CreatorID = 5,
			UpdaterID = 6,
			WorkflowID = 7,
			StateID = 8,
			Type = 9,
			ArchiveID = 10,
			IsParent = 11,
			Name = 12,
			Desc = 13,
			Code = 14,
			Version = 15,
			CreateTime = 16,
			UpdateTime = 17,
			Config = 18,
			Table = 19,
			EnvironmentID = 21,
			ParentID = 22,
			MgrType = 23,
			Access = 24,
			ProjGuid = 25,
			PprjGuid = 26,
			WSpaceProfID = 27,
			ComponentClassID = 28,
			LocationID = 29,
			Flags = 30,
			ComponentInstanceID = 31,
			LocationSource = 32
		}

		public enum ReferenceInfoProperty
		{
			ElemID = 1,
			MasterGuid,
			MasterModelID,
			ReferenceGuid,
			ReferenceModelID,
			NestDepth,
			ReferenceType,
			Flags
		}

		public enum ReferenceInfoFlags
		{
			DisplayRaster = 2,
			RefHasMoved = 4
		}

		public enum EnvironmentProperty
		{
			ID = 1,
			TableID,
			Flags,
			AttrNo,
			Name,
			Desc
		}

		public enum EnvAttributeGUIProperty
		{
			EnvironmentID = 1,
			TableID,
			ColumnID,
			GUIID,
			PageNo,
			TabOrder,
			LableFontHeight,
			LabelX,
			LabelY,
			LabelWidth,
			LabelHeight,
			EditX,
			EditY,
			EditWidth,
			EditHeight,
			GUIFlags,
			Label,
			LabelFont,
			Prompt
		}

		public enum ApplicationProperty
		{
			ID = 1,
			Name
		}

		public enum UserProperty
		{
			ID = 1,
			Name,
			Desc,
			Password,
			Email,
			Type,
			SecProvider,
			Flags,
			CreateDate
		}

		public enum DepartmentProperty
		{
			ID = 1,
			Name,
			Desc,
			DisplayName
		}

		public enum StateProperty
		{
			ID = 1,
			Name,
			Desc
		}

		public enum StorageProperty
		{
			ID = 1,
			Name,
			Desc,
			Node,
			Path,
			Protocol,
			DisplayName
		}

		public enum WorkflowProperty
		{
			ID = 1,
			Type,
			Name,
			Desc
		}

		public enum SavedQueryProperty
		{
			QueryId = 1,
			UserId,
			PQueryId,
			HasCriteria,
			HasSubItems,
			PropName,
			PropDesc
		}

		public enum ObjectTypes
		{
			UserIgnoresAccessControl,
			EnvProject,
			Project,
			EnvDoc,
			Document
		}

		public enum ObjectTypeForLinkData
		{
			None,
			DocumentByProject,
			Document,
			DocumentByWorkspace,
			DocumentBySet,
			DocumentByAttrRec
		}

		public enum VaultType
		{
			Normal,
			Workspace
		}

		public enum DocumentType
		{
			Normal = 10,
			History = 11,
			Set = 12,
			Redline = 13,
			ModelerBRP = 14,
			Abstract = 15,
			Unknown = 0
		}

		public enum AttributeLinkageType
		{
			Document = 1,
			Environment
		}

		public enum LinkProperty
		{
			VaultID = 1,
			DocumentID,
			TableID,
			ColumnID,
			ColumnValue,
			DocGuid
		}

		public enum LinkDataProperty
		{
			TableID = 1,
			ColumnID,
			ColumnType,
			ColumnLength,
			ColumnName,
			ColumnFormat,
			ColumnDescription,
			ColumnNativeType
		}

		public enum InterfaceProperty
		{
			ID = 1,
			Name
		}

		[Flags]
		public enum DocumentCopyFlags : uint
		{
			CanOverwrite = 1u,
			Attrs = 2u,
			NoAttrs = 4u,
			NoSetItem = 8u,
			Move = 0x10u,
			NoFile = 0x20u,
			NoHooks = 0x10000u,
			LogMove = 0x20000u,
			IncludeVersions = 0x40000u
		}

		public enum VaultDescriptorFlags : uint
		{
			VaultID = 1u,
			EnvironmentID = 2u,
			ParentID = 4u,
			StorageID = 8u,
			ManagerID = 0x10u,
			TypeID = 0x20u,
			Workflow = 0x40u,
			Name = 0x80u,
			Description = 0x100u,
			Configuration = 0x200u,
			ManagerType = 0x400u,
			WSpaceProfID = 0x800u
		}

		public enum CodeDefinitionType
		{
			All = -1,
			PartOfCode = 1,
			DocumentCodePlaceHolder = 2,
			AdditionalDocumentCode = 3,
			RevisionPlaceHolder = 4
		}

		public enum DocumentCodeDefinitionProperty
		{
			EnvironmentID = 1,
			TableID,
			ColumnID,
			Type,
			SerialType,
			Flags,
			OrderNumber,
			ConnectString
		}

		public enum DocumentCodeDefinitionFlag : uint
		{
			AllowEmpty = 2u
		}

		public enum DocumentCodeSerialType
		{
			None,
			Number,
			UsedWith
		}

		public enum AttributeDefinitionProperty
		{
			EnvironmentID = 1,
			TableID,
			ColumnID,
			ControlType,
			EditFontHeight,
			FieldFlags,
			DefaultValueType,
			FieldLength,
			FieldAccess,
			ValueListType,
			EditFont,
			DefaultValue,
			FieldFormat,
			ValueListSource,
			Extra1,
			Extra2,
			Extra3,
			Extra4,
			Extra5
		}

		public enum AccessControlProperty
		{
			ObjectType = 1,
			ObjectId1,
			ObjectId2,
			Workflow,
			State,
			MemberType,
			MemberId,
			AccessMask,
			ObjectGuid
		}

		public enum GroupProperty
		{
			ID = 1,
			Name,
			Description,
			Type,
			SecurityProvider
		}

		public enum AttributeParameterFlags
		{
			Unique = 1,
			Required = 2,
			EditableIfFinal = 0x10,
			MultiSelect = 0x40,
			LimitToList = 0x80,
			CopyClearNewSheet = 0x1000,
			CopyClearInEnvironment = 0x2000,
			CopyClearOutEnvironment = 0x4000,
			CopyClearOutDatabase = 0x8000
		}

		public enum AttributeDefaultValueTypes
		{
			None,
			Fixed,
			Select,
			SystemVariable,
			Function
		}

		public enum TableProperty
		{
			ID = 1,
			Type,
			Name,
			Description
		}

		public enum ColumnProperty
		{
			ColumnID = 1,
			TableID,
			SQLDataType,
			Precision,
			Scale,
			Type,
			Length,
			Unique,
			Name,
			Description,
			FormatString
		}

		public enum SqlDataType
		{
			Unknown = 0,
			Numeric = 1,
			Decimal = 2,
			Integer = 3,
			SmallInt = 4,
			Float = 5,
			Real = 6,
			Double = 7,
			DateTime = 8,
			Char = 9,
			VarChar = 10,
			LongVarChar = 11,
			WChar = 12,
			VarWChar = 13,
			LongVarWChar = 14,
			Date = 15,
			Time = 16,
			TimeStamp = 17,
			Binary = 18,
			VarBinary = 19,
			LongVarBinary = 20,
			Guid = 21,
			Bit = 22,
			BigInt = 23,
			TinyInt = 26
		}

		public enum ColumnDataType
		{
			Unknown = 0,
			Character = 9,
			Long = 3,
			Short = 4,
			Float = 6,
			Double = 7,
			Date = 15,
			Time = 16,
			TimeStamp = 17,
			Binary = 18,
			WChar = 12,
			Guid = 21,
			Boolean = 22
		}

		public enum AADMSBUFFER_SPATIALLOCATION
		{
			AASPATIALLOCATION_ACCESS_RIGHTS = 1,
			AASPATIALLOCATION_SOURCE,
			AASPATIALLOCATION_SRS_GUID,
			AASPATIALLOCATION_MSRS_GUID,
			AASPATIALLOCATION_GEOMETRY,
			AASPATIALLOCATION_MSRS_GEOMETRY,
			AASPATIALLOCATION_LOCATION_GUID,
			AASPATIALLOCATION_SPATIAL_OBJECT
		}

		public enum AADMSBUFFER_DOCUMENT
		{
			DOC_PROP_ID = 1,
			DOC_PROP_VERSIONNO = 2,
			DOC_PROP_PROPOSALNO = 3,
			DOC_PROP_CREATORID = 4,
			DOC_PROP_UPDATERID = 5,
			DOC_PROP_USERID = 6,
			DOC_PROP_SIZE = 7,
			DOC_PROP_FILETYPE = 8,
			DOC_PROP_ITEMTYPE = 9,
			DOC_PROP_STORAGEID = 10,
			DOC_PROP_SETID = 11,
			DOC_PROP_SETTYPE = 12,
			DOC_PROP_WORKFLOWID = 13,
			DOC_PROP_STATEID = 14,
			DOC_PROP_APPLICATIONID = 15,
			DOC_PROP_DEPARTMENTID = 16,
			DOC_PROP_ORIGINALNO = 18,
			DOC_PROP_IS_OUT_TO_ME = 19,
			DOC_PROP_NAME = 20,
			DOC_PROP_FILENAME = 21,
			DOC_PROP_DESC = 22,
			DOC_PROP_VERSION = 23,
			DOC_PROP_CREATE_TIME = 24,
			DOC_PROP_UPDATE_TIME = 25,
			DOC_PROP_DMSSTATUS = 26,
			DOC_PROP_DMSDATE = 27,
			DOC_PROP_NODE = 28,
			DOC_PROP_PROJECTID = 29,
			DOC_PROP_ACCESS = 30,
			DOC_PROP_IS_LOGICALSET_MASTER = 31,
			DOC_PROP_IS_REDLINE_MASTER = 32,
			DOC_PROP_IS_REF_MASTER = 33,
			DOC_PROP_IS_SATELLITE_MASTER = 34,
			DOC_PROP_HAS_FINAL_STATUS = 35,
			DOC_PROP_MANAGER = 36,
			DOC_PROP_FILE_UPDATER = 37,
			DOC_PROP_LAST_RT_LOCKER = 38,
			DOC_PROP_ITEM_FLAGS = 39,
			DOC_PROP_FILE_UPDATE_TIME = 40,
			DOC_PROP_LAST_RT_LOCK_TIME = 41,
			DOC_PROP_MGRTYPE = 44,
			DOC_PROP_IS_URL = 45,
			DOC_PROP_URL_NAME = 46,
			DOC_PROP_DOCGUID = 47,
			DOC_PROP_PROJGUID = 48,
			DOC_PROP_ORIGGUID = 49,
			DOC_PROP_WSPACEPROFID = 50,
			DOC_PROP_IS_3D_FILE = 51,
			DOC_PROP_IS_2D_FILE = 52,
			DOC_PROP_FILE_REVISION = 53,
			DOC_PROP_OVERLAPS = 54,
			DOC_PROP_LOCATIONID = 55,
			DOC_PROP_MIMETYPE = 56,
			DOC_PROP_LOCATIONSOURCE = 58
		}

		public enum SqlSelectProperty
		{
			ColumnType = 1,
			ColumnNativeType,
			ColumnLength,
			ColumnName
		}

		public enum UserNumericSetting
		{
			DisplayShowVersion = 151
		}

		public enum eQueryResultFlags
		{
			Unfiltered,
			NoDocVersions,
			VersionsBySetting
		}

		public struct VaultDescriptor
		{
			public uint Flags;

			public int VaultID;

			public int EnvironmentID;

			public int ParentID;

			public int StorageID;

			public int ManagerID;

			public int TypeID;

			public int WorkflowID;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Name;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Description;

			public int ManagerType;

			public int WorkspaceProfileId;

			public Guid GuidVault;

			public int ComponentClassId;

			public int ComponentInstanceId;

			public uint ProjFlagMask;

			public uint ProjFlags;
		}

		public struct AADMSPROJGUIDTITEM
		{
			public uint flags;

			public Guid projectGuid;

			public Guid parentGuid;

			public int environmentID;

			public int storageID;

			public int managerID;

			public int typeID;

			public int workflowID;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string projectName;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string projectDescription;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string configString;

			public int managerType;

			public int workspaceProfileId;

			public int componentClassId;

			public int componentInstanceId;
		}

		[Flags]
		public enum AADMSPROJGUIDTITEM_flags
		{
			AADMSPROJGF_PROJGUID = 1,
			AADMSPROJGF_ENVID = 2,
			AADMSPROJGF_PARENTGUID = 4,
			AADMSPROJGF_STORAGEID = 8,
			AADMSPROJGF_MANAGERID = 0x10,
			AADMSPROJGF_TYPEID = 0x20,
			AADMSPROJGF_WORKFLOW = 0x40,
			AADMSPROJGF_NAME = 0x80,
			AADMSPROJGF_DESC = 0x100,
			AADMSPROJGF_CONFIG = 0x200,
			AADMSPROJGF_MGRTYPE = 0x400,
			AADMSPROJGF_WORKSPACEPROFILE = 0x800,
			AADMSPROJGF_COMPONENT_CLASSID = 0x1000,
			AADMSPROJGF_COMPONENT_INSTANCEID = 0x2000,
			AADMSPROJGF_REQUIREDONCREATE = 0x88,
			AADMSPROJGF_ALL = 0x3FFF
		}		

		[Flags]
		public enum AADMSProjFlags
		{
			ProjectId = 1,
			Envid = 2,
			ParentId = 4,
			StorageId = 8,
			ManagerId = 16,
			TypeId = 32,
			Workflow = 64,
			Name = 128,
			Desc = 256,
			Mgrtype = 1024,
			WspaceProfId = 2048,
			Guid = 4096,
			ComponentClassId = 8192,
			ProjFlags = 16384,
			ComponentInstanceId = 32768,
			RequiredOnCreate = StorageId | Name, // Combine flags
			All = 65535 // All flags combined
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct AADMSPROJITEM
		{
			public uint ulFlags;

			public int lProjectId;

			public int lEnvironmentId;

			public int lParentId;

			public int lStorageId;

			public int lManagerId;

			public int lTypeId;

			public int lWorkflowId;

			public string lptstrName;

			public string lptstrDesc;

			public int lManagerType;

			public int lWorkspaceProfileId;

			public IntPtr guidVault;

			public int lComponentClassId;

			public int lComponentInstanceId;

			public uint projFlagMask;

			public uint projFlags;

			public AADMSPROJITEM(string projname, string projdesc)
			{
				this.ulFlags = 0;
				this.lProjectId = 0;
				this.lEnvironmentId = 0;
				this.lParentId = 0;
				this.lStorageId = 0;
				this.lManagerId = 0;
				this.lTypeId = 0;
				this.lWorkflowId = 0;
				this.lptstrName = projname;
				this.lptstrDesc = projdesc;
				this.lManagerType = 0;
				this.lWorkspaceProfileId = 0;
				this.guidVault = IntPtr.Zero;
				this.lComponentClassId = 0;
				this.lComponentInstanceId = 0;
				this.projFlagMask = 0;
				this.projFlags = 0;
			}
		}

		public struct DocCreateOutput
		{
			public uint Mask;

			private string WorkingFile;

			private int BufferSize;

			private int AttributeId;
		}

		public enum AccessMasks
		{
			None = 0,
			Control = 1,
			Write = 2,
			Read = 4,
			FWrite = 8,
			FRead = 16,
			Create = 32,
			Delete = 64,
			Free = 128,
			ChangeWorkflowState = 256,
			Owner = 512,
			Full = 65535
		}

		public enum ManagerTypes
		{
			Any = -1,
			User = 1,
			Group = 2,
			UserList = 3,
			AllUsers = 4
		}

		[Flags]
		public enum DocumentCreationFlag : uint
		{
			Default = 0u,
			NoAttributeRecord = 1u,
			CreateAttributeRecord = 2u,
			NoAuditTrail = 4u
		}

		public enum DocumentDeleteMasks
		{
			None = 0,
			NoSetChild = 1,
			NoSetParent = 2,
			MoveAction = 4,
			IncludeVersions = 8
		}

		[Flags]
		public enum LinkDataSelectFlags : uint
		{
			Creator = 1u,
			CreateTime = 2u,
			Updater = 4u,
			UpdateTime = 8u,
			SysColsOnly = 0x10u,
			DocGuid = 0x20u
		}

		[Flags]
		public enum NewVersionCreationFlags : uint
		{
			None = 0u,
			CopyAttrs = 1u,
			KeepRelations = 2u
		}

		[Flags]
		public enum DocumentFileFlags : uint
		{
			DocumentFileReplace = 0u,
			DocumentFileRename = 1u
		}

		[Flags]
		public enum MonikerFlags : uint
		{
			ResourceUserSetting = 0u,
			ResourceLocation = 1u,
			ResourceName = 2u,
			ResourceDescription = 4u,
			ResourceMask = 7u,
			DontValidate = 8u,
			Visual = 0x10u,
			MaskValid = 0xFFFFu
		}

		[Flags]
		public enum NestedReferencesScanFlags : uint
		{
			AADMS_REFLIST_FROM_REFINFO = 1u,
			AADMS_REFLIST_FROM_SETINFO = 2u,
			AADMS_REFLIST_ALLOW_SELF_REFS = 4u
		}

		public enum eRestrictionRelation_t
		{
			DMS_RELATION_NONE = 0,
			DMS_RELATION_EQUAL = 1,
			DMS_RELATION_NOTEQUAL = 2,
			DMS_RELATION_LESSTHAN = 3,
			DMS_RELATION_GREATERTHAN = 4,
			DMS_RELATION_GREATEROREQUAL = 5,
			DMS_RELATION_LESSOREQUAL = 6,
			DMS_RELATION_BETWEEN = 7,
			DMS_RELATION_ISNULL = 8,
			DMS_RELATION_ISNOTNULL = 9,
			DMS_RELATION_ISLIKE = 10,
			DMS_RELATION_IN = 11,
			DMS_RELATION_NOTIN = 12,
			DMS_RELATION_INNERJOIN = 13,
			DMS_RELATION_LEFTOUTERJOIN = 14,
			DMS_RELATION_RIGHTOUTERJOIN = 15,
			DMS_RELATION_ISNOTLIKE = 16,
			DMS_RELATION_NOTBETWEEN = 17,
			DMS_RELATION_NODE_OR_SUBNODE = 18,
			DMS_RELATION_DERIVED_TYPE = 19,
			DMS_RELATION_EXPRESSION = 1024,
			DMS_RELATION_INCL_PHRASE = 5000,
			DMS_RELATION_INCL_ANYWORD = 5001,
			DMS_RELATION_INCL_ALLWORDS = 5002,
			DMS_RELATION_INCL_NONEOFWORDS = 5003,
			DMS_RELATION_SUBQUERY_COLUMN = 6001,
			DMS_RELATION_UNION_ALL = 6002,
			DMS_RELATION_SUBORGROUP = 7003,
			DMS_RELATION_SUBANDGROUP = 7004
		}

		public enum eDocumentProperties_t
		{
			NONE = -1,
			QRY_DOC_PROP_ENVIRONMENT_ID = 1,
			QRY_DOC_PROP_PROJ_ID = 2,
			QRY_DOC_PROP_PROJ_NAME = 3,
			QRY_DOC_PROP_PROJ_DESC = 4,
			QRY_DOC_PROP_FILENAME = 5,
			QRY_DOC_PROP_NAME = 6,
			QRY_DOC_PROP_DESC = 7,
			QRY_DOC_PROP_VERSION = 8,
			QRY_DOC_PROP_VERSIONSEQ = 9,
			QRY_DOC_PROP_CREATORID = 10,
			QRY_DOC_PROP_UPDATERID = 11,
			QRY_DOC_PROP_DMSSTATUS = 12,
			QRY_DOC_PROP_LASTUSERID = 13,
			QRY_DOC_PROP_FILETYPE = 14,
			QRY_DOC_PROP_ITEMTYPE = 15,
			QRY_DOC_PROP_STORAGEID = 16,
			QRY_DOC_PROP_WORKFLOWID = 17,
			QRY_DOC_PROP_STATEID = 18,
			QRY_DOC_PROP_APPLICATIONID = 19,
			QRY_DOC_PROP_DEPARTMENTID = 20,
			QRY_DOC_PROP_INCSUBVAULTS = 21,
			QRY_DOC_PROP_FINAL_STATUS = 22,
			QRY_DOC_PROP_FINAL_USER = 23,
			QRY_DOC_PROP_FINAL_DATE = 24,
			QRY_DOC_PROP_LOCATIONID = 25,
			QRY_DOC_PROP_FILE_REVISION = 26,
			QRY_DOC_PROP_OVERLAPS = 27,
			QRY_DOC_PROP_MIMETYPE = 28,
			QRY_DOC_PROP_ID = 101,
			QRY_DOC_PROP_PROPOSALNO = 102,
			QRY_DOC_PROP_SIZE = 104,
			QRY_DOC_PROP_SETID = 105,
			QRY_DOC_PROP_SETTYPE = 106,
			QRY_DOC_PROP_ORIGINALNO = 107,
			QRY_DOC_PROP_IS_OUT_TO_ME = 108,
			QRY_DOC_PROP_CREATE_TIME = 109,
			QRY_DOC_PROP_UPDATE_TIME = 110,
			QRY_DOC_PROP_DMSDATE = 111,
			QRY_DOC_PROP_NODE = 112,
			QRY_DOC_PROP_ACCESS = 113,
			QRY_DOC_PROP_MANAGERID = 114,
			QRY_DOC_PROP_FILE_UPDATERID = 115,
			QRY_DOC_PROP_LAST_RT_LOCKERID = 116,
			QRY_DOC_PROP_ITEM_FLAGS = 117,
			QRY_DOC_PROP_FILE_UPDATE_TIME = 118,
			QRY_DOC_PROP_LAST_RT_LOCK_TIME = 119,
			QRY_DOC_PROP_MGRTYPE = 120,
			QRY_DOC_PROP_DOCGUID = 121,
			QRY_DOC_PROP_PROJGUID = 122,
			QRY_DOC_PROP_ORIGGUID = 123,
			QRY_DOC_PROP_PROJ_VERSIONNO = 124,
			QRY_DOC_PROP_PROJ_MANAGERID = 125,
			QRY_DOC_PROP_PROJ_STORAGEID = 126,
			QRY_DOC_PROP_PROJ_CREATORID = 127,
			QRY_DOC_PROP_PROJ_UPDATERID = 128,
			QRY_DOC_PROP_PROJ_WORKFLOWID = 129,
			QRY_DOC_PROP_PROJ_STATEID = 130,
			QRY_DOC_PROP_PROJ_TYPE = 131,
			QRY_DOC_PROP_PROJ_ARCHIVEID = 132,
			QRY_DOC_PROP_PROJ_ISPARENT = 133,
			QRY_DOC_PROP_PROJ_CODE = 134,
			QRY_DOC_PROP_PROJ_VERSION = 135,
			QRY_DOC_PROP_PROJ_CREATE_TIME = 136,
			QRY_DOC_PROP_PROJ_UPDATE_TIME = 137,
			QRY_DOC_PROP_PROJ_CONFIG = 138,
			QRY_DOC_PROP_PROJ_PARENTID = 140,
			QRY_DOC_PROP_PROJ_MGRTYPE = 141,
			QRY_DOC_PROP_PROJ_ACCESS = 142,
			QRY_DOC_PROP_PROJ_PROJGUID = 143,
			QRY_DOC_PROP_PROJ_PPRJGUID = 144,
			QRY_PROP_ACCUMULATED_TEXTS = 200,
			QRY_PROP_DATASOURCE_GUID = 201,
			QRY_PROP_VIEW_ID = 250,
			QRY_DOC_PROP_CHECKOUT_USERID = 301,
			QRY_DOC_PROP_CHECKOUT_NODE = 302,
			QRY_DOC_PROP_CHECKOUT_COUTTIME = 303,
			QRY_DOC_PROP_LOCATIONSOURCE = 304,
			QRY_DOC_PROP_PROJ_COMPONENT_CLASSID = 305,
			QRY_DOC_PROP_PROJ_COMPONENT_INSTANCEID = 306,
			QRY_DOC_PROP_PROJ_LOCATIONID = 307,
			QRY_DOC_PROP_PROJ_LOCATIONSOURCE = 308,
			QRY_DOC_PROP_PROJ_WSPACEPROFID = 309
		}

		public enum AttributeDataType
		{
			UNDEFINED = -1,
			AADMS_ATTRFORM_DATATYPE_STRING = 1,
			AADMS_ATTRFORM_DATATYPE_INT = 2,
			AADMS_ATTRFORM_DATATYPE_UINT = 3,
			AADMS_ATTRFORM_DATATYPE_FLOAT = 4,
			AADMS_ATTRFORM_DATATYPE_DATE_TO_DAY = 5,
			AADMS_ATTRFORM_DATATYPE_DATE_TO_SEC = 6,
			AADMS_ATTRFORM_DATATYPE_STRING_AS_DATE_TO_DAY = 7,
			AADMS_ATTRFORM_DATATYPE_STRING_AS_DATE_TO_SEC = 8,
			AADMS_ATTRFORM_DATATYPE_GUID = 9,
			AADMS_ATTRFORM_DATATYPE_BIGINT = 10,
			AADMS_ATTRFORM_DATATYPE_UBIGINT = 11,
			AADMS_ATTRFORM_DATATYPE_TIMESPAN = 12
		}

		public struct BindRequest
		{
			public uint colCount;

			private SqlBindColumn BindCols;
		}

		public struct SqlBindColumn
		{
			public ColumnDataType sqlctype;

			public short buflen;
		}

		public struct FindDocResults
		{
			public int dwRowCount;

			public IntPtr pRow;
		}

		public struct FindDocResult
		{
			public int dwColumnCount;

			public IntPtr pCol;
		}

		public struct FindDocResultCol
		{
			public int dwType;

			public int _padding1;

			public int lValue;

			public int _padding2;
		}

		public struct FindDocResultStringCol
		{
			public int dwType;

			public int _padding1;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string strValue;

			public int _padding2;
		}

		public struct FindDocResultPtrCol
		{
			public int dwType;

			public int _padding1;

			public IntPtr ptrValue;

			public int _padding2;
		}

		public struct EnvironmentAttributeDocumentLinkage
		{
			public int type;

			public int projectId;

			public int documentId;
		}

		public struct DocumentProperties_stc
		{
			public int dwPropertyCount;

			public eQueryResultFlags eResultFlags;

			public DocumentProperty_stc[] sPropertyArray;
		}

		public struct DocumentProperty_stc
		{
			public Guid guidPropertySet;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string lpctstrPropertyName;

			public int dwPropertyID;

			public int dwResultType;
		}

		public struct ItemReference
		{
			public Guid typeSystem;

			public string typeName;

			public int typeId;

			public string relationName;
		}

		public struct ItemPropertyReference
		{
			public ItemReference item;

			public string propertyName;

			public int propertyId;

			public int dataType;
		}

		public struct SearchContext
		{
			public ItemReference fromType;

			public ItemPropertyReference[] resultColumns;
		}

		[Flags]
		public enum FindSerialNumberAlgorithmFlag : uint
		{
			DoSequence = 1u,
			UserRange = 2u,
			Hole = 4u
		}

		public struct FindSerialNumberRules
		{
			public FindSerialNumberAlgorithmFlag FindParameters;

			public uint SerialLowerLimit;

			public uint SerialUpperLimit;
		}

		public enum SpecialLinkDataColumnIdentifiers
		{
			VaultID = -2,
			DocumentID = -3,
			UniqueValue = -4,
			CreatorID = -5,
			CreateTime = -6,
			UpdaterID = -7,
			UpdateTime = -8,
			DocGUID = -9
		}

		public enum AuditTrailTypes
		{
			NONE = 0,
			AADMSAT_TYPE_FIRST = 1,
			AADMSAT_TYPE_VAULT = 1,
			AADMSAT_TYPE_DOCUMENT = 2,
			AADMSAT_TYPE_DOCUMENT_SET = 3,
			AADMSAT_TYPE_WORKFLOW = 4,
			AADMSAT_TYPE_STATE = 5,
			AADMSAT_TYPE_USER = 6,
			AADMSAT_TYPE_LAST = 6
		}

		public enum AuditTrailActions
		{
			AADMSAT_ACT_DOC_FIRST = 1000,
			AADMSAT_ACT_DOC_UNKNOWN = 1000,
			AADMSAT_ACT_DOC_CREATE = 1001,
			AADMSAT_ACT_DOC_MODIFY = 1002,
			AADMSAT_ACT_DOC_ATTR = 1003,
			AADMSAT_ACT_DOC_FILE_ADD = 1004,
			AADMSAT_ACT_DOC_FILE_REM = 1005,
			AADMSAT_ACT_DOC_FILE_REP = 1006,
			AADMSAT_ACT_DOC_CIN = 1007,
			AADMSAT_ACT_DOC_VIEW = 1008,
			AADMSAT_ACT_DOC_CHOUT = 1009,
			AADMSAT_ACT_DOC_CPOUT = 1010,
			AADMSAT_ACT_DOC_GOUT = 1011,
			AADMSAT_ACT_DOC_STATE = 1012,
			AADMSAT_ACT_DOC_FINAL_S = 1013,
			AADMSAT_ACT_DOC_FINAL_R = 1014,
			AADMSAT_ACT_DOC_VERSION = 1015,
			AADMSAT_ACT_DOC_MOVE = 1016,
			AADMSAT_ACT_DOC_COPY = 1017,
			AADMSAT_ACT_DOC_SECUR = 1018,
			AADMSAT_ACT_DOC_REDLINE = 1019,
			AADMSAT_ACT_DOC_DELETE = 1020,
			AADMSAT_ACT_DOC_EXPORT = 1021,
			AADMSAT_ACT_DOC_FREE = 1022,
			AADMSAT_ACT_DOC_EXTRACT = 1023,
			AADMSAT_ACT_DOC_DISTRIBUTE = 1024,
			AADMSAT_ACT_DOC_LAST = 1024
		}

		public enum AuditTrailDates
		{
			AADMSAT_DATE_ANYTIME,
			AADMSAT_DATE_CUSTOM,
			AADMSAT_DATE_YESTERDAY,
			AADMSAT_DATE_TODAY,
			AADMSAT_DATE_7_DAYS,
			AADMSAT_DATE_LAST_WEEK,
			AADMSAT_DATE_THIS_WEEK,
			AADMSAT_DATE_LAST_MONTH,
			AADMSAT_DATE_THIS_MONTH
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Anonymous_c43ebf7d_f500_4c9b_8a11_08b49d5f4fc8
		{
			[FieldOffset(0)]
			public Guid idGUID;

			[FieldOffset(0)]
			public int lId;
		}

		public struct AuditTrailObject_stc
		{
			public AuditTrailTypes lObjectType;

			public Anonymous_c43ebf7d_f500_4c9b_8a11_08b49d5f4fc8 Union1;
		}

		public struct AuditTrailCriteria_stc
		{
			public AuditTrailTypes lObjectType;

			public int lObjectCount;

			public AuditTrailObject_stc[] lpObjects;

			public bool bRecursive;

			public bool bIncludeChildObjects;

			public int lActionCount;

			public AuditTrailActions[] lplActions;

			public AuditTrailDates ulDateTimeType;

			public string lpctstrStartDate;

			public string lpctstrEndDate;

			public int lUserCount;

			public int[] lplUsers;
		}

		public struct AuditTrailCriteriaFake_stc
		{
			public AuditTrailTypes lObjectType;

			public int lObjectCount;

			public IntPtr lpObjects;

			public bool bRecursive;

			public bool bIncludeChildObjects;

			public int lActionCount;

			public IntPtr lplActions;

			public int ulDateTimeType;

			public string lpctstrStartDate;

			public string lpctstrEndDate;

			public int lUserCount;

			public IntPtr lplUsers;
		}

		[Flags]
		public enum DocFetchFlags : uint
		{
			CheckOut = 0u,
			Export = 1u,
			CopyOut = 2u,
			Refresh = 4u,
			Lock = 8u,
			UseUpToDateCopy = 0x10u,
			AcceptCheckouts = 0x20u,
			CopyOutMaster = 0x40u,
			AsSetMembers = 0x1000u,
			ExportReferences = 0x2000u,
			ChangeSetId = 0x4000u,
			UseVaultDirs = 0x8000u,
			IgnoreMaster = 0x10000u,
			GiveOut = 0x20000u,
			View = 0x40002u,
			NoAuditTrail = 0x80000u,
			MasterAsSet = 0x10000000u,
			IgnoreRedlineRelations = 0x20000000u,
			FetchNestedReferences = 0x40000000u,
			FetchRedlinesReferences = 0x80000000u
		}

		[Flags]
		public enum DocPushFlags : uint
		{
			LeaveCopy = 1u,
			UpdateServer = 2u,
			DeleteFiles = 4u,
			Import = 8u,
			Unmodified = 0x10u,
			AllMembers = 0x1000u,
			CheckIfUsedExternally = 0x2000u,
			TreatAsUptodate = 0x8000u,
			AllowOverlaps = 0x10000u,
			MasterAsSet = 0x10000000u
		}

		public enum AccessCarrier
		{
			Invalid,
			Document,
			Folder,
			Environment,
			DataSource,
			WorkFlow,
			WorkFlowState,
			Global,
			GranularSecurity
		}

		public enum SubItemAccessFlags
		{
			Unchanged = 0,
			StripExportProjects = 14,
			StripAll = 15
		}

		public struct ObjectAccessEntry
		{
			public int workflowId;

			public int stateId;

			public ManagerTypes memberType;

			public int memberId;

			public AccessMasks accessMask;
		}

		public struct ObjectAccessSet
		{
			public ObjectTypes objectType;

			public uint entryCount;

			public IntPtr entries;
		}

		public struct SQLTimeStamp_stc
		{
			public short year;

			public ushort month;

			public ushort day;

			public ushort hour;

			public ushort minute;

			public ushort second;

			public int fraction;
		}

		public struct dms_combined_access_stc
		{
			public int o_aclno;

			public ObjectTypes o_objtype;

			public Guid o_objguid;

			public int o_objno;

			public int o_objno2;

			public int o_workflowno;

			public int o_stateno;

			public ManagerTypes o_memtype;

			public int o_memberno;

			public AccessMasks o_mask;

			public SQLTimeStamp_stc o_updatetime;
		}

		public enum AADMSFOAF
		{
			Ignore_Parents = 1,
			Ignore_Env = 2,
			Ignore_Default = 4,
			ExactMatch = 8,
			IgnoreUserSettings = 0x10,
			AllWFLStates = 0x20,
			Ignore_Objacce = 0x10000
		}

		public enum AcceProperty
		{
			ObjType = 1,
			ObjID1,
			ObjID2,
			WorkFlow,
			State,
			MemType,
			MemId,
			AcceMask,
			ObjGuid
		}

		[Flags]
		public enum ProjectOperationFlags
		{
			AAPRO_ARRAY_EXCLUDE_PARENT = 1,
			AAPRO_ARRAY_NO_DOCUMENTS = 2,
			AAPRO_ARRAY_NO_SETS = 4,
			AAPRO_ARRAY_NO_RECURSIO = 8,
			AAPRO_ARRAY_ATTRIBUTES = 0x10,
			AAPRO_ARRAY_NO_PROJECTS = 0x20,
			AAPRO_ARRAY_TAKE_OWNERSHIP = 0x40,
			AAPRO_ARRAY_ALLOW_COPY_ALL = 0x80,
			AAPRO_ARRAY_NO_CHECKED_OUT = 0x100,
			AAPRO_ARRAY_SET_REFERENCES = 0x200,
			AAPRO_ARRAY_OWN_CHECK_OUTS = 0x400,
			AAPRO_ARRAY_NO_ACTIVE_VER = 0x800,
			AAPRO_ARRAY_DEL_MWP_VARS = 0x1000,
			AAPRO_ARRAY_COPY_WORKFLOW = 0x1000,
			AAPRO_ARRAY_COPY_ACCESS = 0x2000,
			AAPRO_ARRAY_COPY_MANAGER = 0x4000,
			AAPRO_ARRAY_COPY_STORAGE = 0x8000,
			AAPRO_ARRAY_COPY_ENV = 0x10000,
			AAPRO_ARRAY_COPY_VERSIONS = 0x20000,
			AAPRO_ARRAY_COMPONENTS = 0x40000,
			AAPRO_ARRAY_COPY_SAVED_SRC = 0x80000,
			AAPRO_ARRAY_COPY_RESOURCES = 0x100000,
			AAPRO_ARRAY_COPY_CONFBLOCKS = 0x200000,
			AAPRO_ARRAY_COPY_CONTENTS = 0x400000,
			AAPRO_ARRAY_COPY_WS_PROFL = 0x800000,
			AAPRO_ARRAY_EXP_EMPTY_PRO = 0x100000,
			AAPRO_ARRAY_EXP_SUBF_ROOT = 0x200000,
			AAPRO_ARRAY_EXP_PRJ_DESCR = 0x400000,
			AAPRO_ARRAY_EXP_REF_2_MST = 0x800000,
			AAPRO_ARRAY_EXP_GIVE_OUT = 0x1000000,
			AAPRO_ARRAY_EXP_OUTER_REFS = 0x2000000,
			AAPRO_ARRAY_EXP_REWRITE_REF = 0x4000000,
			AAPRO_ARRAY_EXP_SHARED = 0x8000000
		}

		public delegate int CopyProjDelegate();

		public enum CustomVersionRecordIdentifiers
		{
			AADMS_VERSION_TYPE_FIRST_INTERNAL = 1000,
			AADMS_VERSION_TYPE_CHHIST = 1001,
			AADMS_VERSION_TYPE_DSSET = 1002,
			AADMS_VERSION_TYPE_AMSG = 1003,
			AADMS_VERSION_TYPE_ATTRMAP = 1004,
			AADMS_VERSION_TYPE_REGTABLES = 1005,
			AADMS_VERSION_TYPE_ODS = 1006,
			AADMS_VERSION_TYPE_ODS85 = 1007,
			AADMS_VERSION_TYPE_DGNINDEXING = 1008,
			AADMS_VERSION_TYPE_BUILDINGINDEXING = 1009,
			AADMS_VERSION_TYPE_BUILDINGDGINDEXING = 1010,
			AADMS_VERSION_TYPE_MANAGEDWORKSPACE = 1011,
			AADMS_VERSION_TYPE_BENTLEY_ODS_ECSCHEMA = 1012,
			AADMS_VERSION_TYPE_FIRST = 1100,
			AADMS_VERSION_TYPE_LAST = 5000
		}

		public enum UserSettings
		{
			AADMS_PAR = 100,
			AADMS_PAR_GEN = 110,
			AADMS_PAR_GEN_USE_AUDIT_TRAIL = 111,
			AADMS_PAR_GEN_USE_ACCESS_CONTROL = 112,
			AADMS_PAR_GEN_EXP_POLICY = 113,
			ADMS_PAR_GEN_LAST = 114,
			AADMS_PAR_WRK_WORKING_DIRECTORY = 171
		}

		public enum UserSessionExpirationTypes
		{
			AADMS_LOGIN_EXP_SERVER_DEFAULT = -1,
			AADMS_LOGIN_EXP_NO_EXPIRATION
		}

		public delegate bool AAPROC_RECONNECT(ulong sessionHandle);

		public struct SpatialDmsObject
		{
			public int typeSystem;

			public int objectType;

			public Guid objectGuid;
		}

		public struct WKPoint3d
		{
			public double x;

			public double y;

			public double z;
		}

		public enum WKGeometryType
		{
			WKTYPE_Unknown,
			WKTYPE_Point,
			WKTYPE_LineString,
			WKTYPE_Polygon,
			WKTYPE_MultiPoint,
			WKTYPE_MultiLineString,
			WKTYPE_MultiPolygon,
			WKTYPE_GeometryCollection,
			WKTYPE_Extrusion
		}

		[Flags]
		public enum ModuleFlags : uint
		{
			AAMODULEF_NON_UI_MODE = 1u
		}

		public static readonly Guid TYPESYSTEM_DMS;

		public static readonly Guid PSET_DOCUMENT_GENERIC;

		public static readonly Guid PSET_ATTRIBUTE_GENERIC;

		static dmscli()
		{
			TYPESYSTEM_DMS = new Guid(1286754707u, 38712, 19089, 161, 240, 200, 20, 148, 168, 89, 121);
			PSET_DOCUMENT_GENERIC = new Guid(1313026314, 17700, 17431, 174, 110, 215, 59, 215, 121, 97, 35);
			PSET_ATTRIBUTE_GENERIC = new Guid(1406941080u, 43260, 16730, 158, 191, 35, 112, 223, 25, 32, 49);
			Util.AppendProjectWiseDllPathToEnvironmentPath();
		}

		public static IntPtr MarshalDocumentPropertysStruct(DocumentProperties_stc DocProps)
		{
			int num = 0;
			for (int i = 0; i < DocProps.dwPropertyCount; i++)
			{
				if (DocProps.sPropertyArray[i].lpctstrPropertyName != null)
				{
					num += (DocProps.sPropertyArray[i].lpctstrPropertyName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
				}
			}
			int num2 = Marshal.SizeOf(typeof(int)) * 2 + Marshal.SizeOf(typeof(DocumentProperty_stc)) * DocProps.sPropertyArray.Length;
			IntPtr intPtr = Marshal.AllocCoTaskMem(num2 + num);
			Marshal.WriteInt32(intPtr, DocProps.dwPropertyCount);
			Marshal.WriteInt32(intPtr, Marshal.SizeOf(typeof(int)), Convert.ToInt32(DocProps.eResultFlags));
			int num3 = num2;
			IntPtr[] array = new IntPtr[DocProps.dwPropertyCount];
			for (int j = 0; j < DocProps.dwPropertyCount; j++)
			{
				if (DocProps.sPropertyArray[j].lpctstrPropertyName != null)
				{
					char[] array2 = DocProps.sPropertyArray[j].lpctstrPropertyName.ToCharArray();
					Marshal.Copy(array2, 0, (IntPtr)(intPtr.ToInt64() + num3), array2.Length);
					ref IntPtr reference = ref array[j];
					reference = (IntPtr)(intPtr.ToInt64() + num3);
					num3 += array2.Length * Marshal.SizeOf(typeof(short));
					Marshal.WriteInt16((IntPtr)(intPtr.ToInt64() + num3), 0);
					num3 += Marshal.SizeOf(typeof(short));
				}
				else
				{
					ref IntPtr reference2 = ref array[j];
					reference2 = IntPtr.Zero;
				}
			}
			int num4 = Marshal.SizeOf(typeof(DocumentProperty_stc));
			num3 = Marshal.SizeOf(typeof(int)) * 2;
			for (int k = 0; k < DocProps.dwPropertyCount; k++)
			{
				Marshal.StructureToPtr((object)DocProps.sPropertyArray[k].guidPropertySet, (IntPtr)(intPtr.ToInt64() + num3), fDeleteOld: false);
				Marshal.WriteIntPtr((IntPtr)(intPtr.ToInt64() + num3), Marshal.SizeOf(typeof(Guid)), array[k]);
				Marshal.WriteInt32((IntPtr)(intPtr.ToInt64() + num3), Marshal.SizeOf(typeof(Guid)) + Marshal.SizeOf(typeof(IntPtr)), DocProps.sPropertyArray[k].dwPropertyID);
				Marshal.WriteInt32((IntPtr)(intPtr.ToInt64() + num3), Marshal.SizeOf(typeof(Guid)) + Marshal.SizeOf(typeof(IntPtr)) + Marshal.SizeOf(typeof(int)), DocProps.sPropertyArray[k].dwResultType);
				num3 += num4;
			}
			return intPtr;
		}

		public static void WriteStringToBuffer(IntPtr Buf, int pointerLocation, string sourceString, ref int iStringOffset)
		{
			if (sourceString != null)
			{
				Marshal.WriteIntPtr(Buf, pointerLocation, (IntPtr)(Buf.ToInt64() + iStringOffset));
				char[] array = sourceString.ToCharArray();
				Marshal.Copy(array, 0, (IntPtr)(Buf.ToInt64() + iStringOffset), array.Length);
				iStringOffset += array.Length * Marshal.SizeOf(typeof(short));
				Marshal.WriteInt16((IntPtr)(Buf.ToInt64() + iStringOffset), 0);
				iStringOffset += Marshal.SizeOf(typeof(short));
			}
			else
			{
				Marshal.WriteIntPtr(Buf, pointerLocation, IntPtr.Zero);
			}
		}

		public static IntPtr MarshalSearchContextStruct(SearchContext sc)
		{
			int num = 0;
			if (sc.fromType.typeName != null)
			{
				num += (sc.fromType.typeName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
			}
			if (sc.fromType.relationName != null)
			{
				num += (sc.fromType.relationName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
			}
			for (int i = 0; i < sc.resultColumns.Length; i++)
			{
				if (sc.resultColumns[i].propertyName != null)
				{
					num += (sc.resultColumns[i].propertyName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
				}
				if (sc.resultColumns[i].item.typeName != null)
				{
					num += (sc.resultColumns[i].item.typeName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
				}
				if (sc.resultColumns[i].item.relationName != null)
				{
					num += (sc.resultColumns[i].item.relationName.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
				}
			}
			int num2 = Marshal.SizeOf(typeof(int));
			int num3 = Marshal.SizeOf(typeof(IntPtr));
			int num4 = num3 * 2 + num2 * 2;
			int num5 = num3 * 4;
			int num6 = num3 * 5;
			int num7 = num6 + num4 * sc.resultColumns.Length;
			int num8 = num7 + num5 * (sc.resultColumns.Length + 1);
			int iStringOffset = num8 + Marshal.SizeOf(typeof(Guid)) * (sc.resultColumns.Length + 1);
			IntPtr intPtr = Marshal.AllocCoTaskMem(iStringOffset + num);
			Marshal.WriteIntPtr(intPtr, 0, (IntPtr)(intPtr.ToInt64() + num7));
			Marshal.WriteIntPtr(intPtr, num3, (IntPtr)0);
			Marshal.WriteIntPtr(intPtr, num3 * 2, IntPtr.Zero);
			Marshal.WriteIntPtr(intPtr, num3 * 3, (IntPtr)sc.resultColumns.Length);
			Marshal.WriteIntPtr(intPtr, num3 * 4, (IntPtr)(intPtr.ToInt64() + num6));
			for (int j = 0; j < sc.resultColumns.Length; j++)
			{
				Marshal.WriteIntPtr(intPtr, num6 + num4 * j, (IntPtr)(intPtr.ToInt64() + num7 + num5 * (j + 1)));
				WriteStringToBuffer(intPtr, num6 + num4 * j + num3, sc.resultColumns[j].propertyName, ref iStringOffset);
				Marshal.WriteInt32(intPtr, num6 + num4 * j + num3 * 2, sc.resultColumns[j].propertyId);
				Marshal.WriteInt32(intPtr, num6 + num4 * j + num3 * 2 + num2, sc.resultColumns[j].dataType);
			}
			Marshal.StructureToPtr((object)sc.fromType.typeSystem, (IntPtr)(intPtr.ToInt64() + num8), fDeleteOld: false);
			Marshal.WriteIntPtr(intPtr, num7, (IntPtr)(intPtr.ToInt64() + num8));
			WriteStringToBuffer(intPtr, num7 + num3, sc.fromType.typeName, ref iStringOffset);
			Marshal.WriteIntPtr(intPtr, num7 + num3 * 2, (IntPtr)sc.fromType.typeId);
			WriteStringToBuffer(intPtr, num7 + num3 * 3, sc.fromType.relationName, ref iStringOffset);
			for (int k = 0; k < sc.resultColumns.Length; k++)
			{
				Marshal.StructureToPtr((object)sc.resultColumns[k].item.typeSystem, (IntPtr)(intPtr.ToInt64() + num8 + Marshal.SizeOf(typeof(Guid)) * (k + 1)), fDeleteOld: false);
				Marshal.WriteIntPtr(intPtr, num7 + num5 * (k + 1), (IntPtr)(intPtr.ToInt64() + num8 + Marshal.SizeOf(typeof(Guid)) * (k + 1)));
				WriteStringToBuffer(intPtr, num7 + num5 * (k + 1) + num3, sc.resultColumns[k].item.typeName, ref iStringOffset);
				Marshal.WriteIntPtr(intPtr, num7 + num5 * (k + 1) + num3 * 2, (IntPtr)sc.resultColumns[k].item.typeId);
				WriteStringToBuffer(intPtr, num7 + num5 * (k + 1) + num3 * 3, sc.resultColumns[k].item.relationName, ref iStringOffset);
			}
			return intPtr;
		}

		public static IntPtr MarshalAuditTrailCriteriaStruct(AuditTrailCriteria_stc atcrit)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			num = Marshal.SizeOf(typeof(AuditTrailObject_stc)) * atcrit.lObjectCount;
			num2 = Marshal.SizeOf(typeof(int)) * atcrit.lActionCount;
			num3 = Marshal.SizeOf(typeof(int)) * atcrit.lUserCount;
			if (atcrit.lpctstrStartDate != null)
			{
				num4 = (atcrit.lpctstrStartDate.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
			}
			if (atcrit.lpctstrEndDate != null)
			{
				num4 += (atcrit.lpctstrEndDate.ToCharArray().Length + 1) * Marshal.SizeOf(typeof(short));
			}
			int num5 = Marshal.SizeOf(typeof(AuditTrailCriteria_stc));
			IntPtr intPtr = Marshal.AllocCoTaskMem(num5 + num + num2 + num3 + num4 + 4);
			AuditTrailCriteriaFake_stc auditTrailCriteriaFake_stc = default(AuditTrailCriteriaFake_stc);
			auditTrailCriteriaFake_stc.lObjectType = atcrit.lObjectType;
			auditTrailCriteriaFake_stc.lObjectCount = atcrit.lObjectCount;
			auditTrailCriteriaFake_stc.bRecursive = atcrit.bRecursive;
			auditTrailCriteriaFake_stc.bIncludeChildObjects = atcrit.bIncludeChildObjects;
			auditTrailCriteriaFake_stc.lActionCount = atcrit.lActionCount;
			auditTrailCriteriaFake_stc.ulDateTimeType = (int)atcrit.ulDateTimeType;
			auditTrailCriteriaFake_stc.lpctstrStartDate = atcrit.lpctstrStartDate;
			auditTrailCriteriaFake_stc.lpctstrEndDate = atcrit.lpctstrEndDate;
			auditTrailCriteriaFake_stc.lUserCount = atcrit.lUserCount;
			auditTrailCriteriaFake_stc.lplUsers = IntPtr.Zero;
			Marshal.StructureToPtr((object)auditTrailCriteriaFake_stc, intPtr, fDeleteOld: true);
			if (0 < atcrit.lObjectCount)
			{
				for (int i = 0; i < atcrit.lObjectCount; i++)
				{
					Marshal.StructureToPtr((object)atcrit.lpObjects[i], (IntPtr)(intPtr.ToInt32() + num5 + Marshal.SizeOf(typeof(AuditTrailObject_stc)) * i), fDeleteOld: true);
				}
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(int)) * 2, (IntPtr)(intPtr.ToInt32() + num5));
			}
			if (0 < atcrit.lActionCount)
			{
				for (int j = 0; j < atcrit.lActionCount; j++)
				{
					Marshal.WriteInt32((IntPtr)(intPtr.ToInt32() + num5 + num + Marshal.SizeOf(typeof(int)) * j), Marshal.SizeOf(typeof(int)), (int)atcrit.lplActions[j]);
				}
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(int)) * 4 + Marshal.SizeOf(typeof(bool)) * 2, (IntPtr)(intPtr.ToInt32() + num5 + num + 4));
			}
			int num6 = num5 + num + num2 + 4;
			if (atcrit.lpctstrStartDate != null)
			{
				char[] array = atcrit.lpctstrStartDate.ToCharArray();
				Marshal.Copy(array, 0, (IntPtr)(intPtr.ToInt32() + num6), array.Length);
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(int)) * 6 + Marshal.SizeOf(typeof(bool)) * 2, (IntPtr)(intPtr.ToInt32() + num6));
				num6 += array.Length * 2;
				Marshal.WriteInt16((IntPtr)(intPtr.ToInt32() + num6), 0);
				num6 += Marshal.SizeOf(typeof(short));
			}
			if (atcrit.lpctstrEndDate != null)
			{
				char[] array2 = atcrit.lpctstrEndDate.ToCharArray();
				Marshal.Copy(array2, 0, (IntPtr)(intPtr.ToInt32() + num6), array2.Length);
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(int)) * 7 + Marshal.SizeOf(typeof(bool)) * 2, (IntPtr)(intPtr.ToInt32() + num6));
				num6 += array2.Length * 2;
				Marshal.WriteInt16((IntPtr)(intPtr.ToInt32() + num6), 0);
				num6 += Marshal.SizeOf(typeof(short));
			}
			if (0 < atcrit.lUserCount)
			{
				for (int k = 0; k < atcrit.lUserCount; k++)
				{
					Marshal.WriteInt32((IntPtr)(intPtr.ToInt32() + num6 + Marshal.SizeOf(typeof(int)) * k), Marshal.SizeOf(typeof(int)), atcrit.lplUsers[k]);
				}
				Marshal.WriteIntPtr(intPtr, Marshal.SizeOf(typeof(int)) * 9 + Marshal.SizeOf(typeof(bool)) * 2, (IntPtr)(intPtr.ToInt32() + num6 + 4));
			}
			return intPtr;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ApplyAccessControlList(AccessCarrier objType, int objectId1, int objectId2, uint typeCount, ObjectAccessSet[] typeAccess, SubItemAccessFlags subItemFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAccessControlItems(ObjectTypes lObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, ManagerTypes lMemberType, int lMemberId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAccessControlItems2(AADMSFOAF ulFlags, ObjectTypes ObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, int ulRequiredMask);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectAccessControlDataBuffer(AADMSFOAF ulFlags, ObjectTypes ObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, int ulRequiredMask);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAccessUsers(AADMSFOAF ulFlags, ObjectTypes lObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, int ulRequiredMask);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern AccessMasks aaApi_GetAccessMaskForUser(ObjectTypes lObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, int lUserId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ModifyAccessItemMask(ObjectTypes lObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, ManagerTypes lMemberType, int lMemberId, AccessMasks lAccessMask);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_RemoveAccessList(ObjectTypes lObjectType, int lObjectId1, int lObjectId2, int lWorkflowId, int lStateId, ManagerTypes lMemberType, int lMemberId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetAccessControlItemNumericProperty(AcceProperty lPropertyId, int lIndex);

		public static bool VerifyAccess(dms_combined_access_stc[] expected)
		{
			int num = expected.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.ObjType, i);
				int num3 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.ObjID1, i);
				int num4 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.ObjID2, i);
				int num5 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.WorkFlow, i);
				int num6 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.State, i);
				int num7 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.MemType, i);
				int num8 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.MemId, i);
				int num9 = aaApi_GetAccessControlItemNumericProperty(AcceProperty.AcceMask, i);
				if (expected[i].o_objtype != (ObjectTypes)num2 || expected[i].o_objno != num3 || expected[i].o_objno2 != num4 || expected[i].o_workflowno != num5 || expected[i].o_stateno != num6 || expected[i].o_memtype != (ManagerTypes)num7 || expected[i].o_memberno != num8 || expected[i].o_mask != (AccessMasks)num9)
				{
					return false;
				}
			}
			return true;
		}

		public static IntPtr MarshalObjectAccessEntries(ObjectAccessEntry[] AccessEntries)
		{
			int num = AccessEntries.Length;
			IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(ObjectAccessEntry)) * num);
			int num2 = Marshal.SizeOf(typeof(ObjectAccessEntry));
			int num3 = 0;
			int num4 = Marshal.SizeOf(typeof(int));
			for (int i = 0; i < num; i++)
			{
				Marshal.WriteInt32(intPtr, num3, AccessEntries[i].workflowId);
				Marshal.WriteInt32(intPtr, num4 + num3, AccessEntries[i].stateId);
				Marshal.WriteInt32(intPtr, num4 * 2 + num3, Convert.ToInt32(AccessEntries[i].memberType));
				Marshal.WriteInt32(intPtr, num4 * 3 + num3, AccessEntries[i].memberId);
				Marshal.WriteInt32(intPtr, num4 * 4 + num3, Convert.ToInt32(AccessEntries[i].accessMask));
				num3 += num2;
			}
			return intPtr;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateProject(ref int createdVaultID, int parentID, int storageID, int managerID, VaultType type, int workflowID, int workspaceProfileID, int copyAccessFromProject, string name, string description);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateProject2(IntPtr LPAADMSPROJITEM, int lAccFromProject);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetProjectIdByNamePath(string lpctstrPath);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDCreateProject(out Guid createdVaultID, ref Guid parentGuid, int storageID, int managerID, VaultType type, int workflowID, int workspaceProfileID, int copyAccessFromProject, string name, string description);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CopyProject(int lSourceProjectId, int lTargetProjectId, ProjectOperationFlags ulFlags, [MarshalAs(UnmanagedType.FunctionPtr)] CopyProjDelegate fpCallBack, int aaUserParam, ref int lplCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteProject(int lSourceProjectId, ProjectOperationFlags ulFlags, [MarshalAs(UnmanagedType.FunctionPtr)] CopyProjDelegate fpCallBack, int aaUserParam, ref int lplCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDDeleteProject(ref Guid lSourceProjectId, ProjectOperationFlags ulFlags, [MarshalAs(UnmanagedType.FunctionPtr)] CopyProjDelegate fpCallBack, int aaUserParam, out int lplCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetVersionRecord(int lVersionType, ref int lplReleaseNo, ref int lplMajorNo, ref int lplMinorNo, ref int lplBuildNo, IntPtr plpctstrDate, IntPtr plpctstrComment);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetVersionRecord(int lVersionType, int lReleaseNo, int lMajorNo, int lMinorNo, int lBuildNo, string lpctstrComment);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_ModifyProject2")]
		public static extern bool aaApi_ModifyProject2_ByVault(ref VaultDescriptor vaultDescriptor);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_ModifyProject2")]
		public static extern bool aaApi_ModifyProject2(IntPtr lpProject);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetParentProject(int lChildId, int lParentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDModifyProject(ref Guid projectGuid, int storageId, int managerId, int projectType, string name, string description);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDModifyProject2([In] ref AADMSPROJGUIDTITEM pProjectItem);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectProject(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectProjectsByStruct(int lProjectId, IntPtr lpCriteria);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GUIDSelectProject(ref Guid guid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectTopLevelProjects();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetProjectStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetProjectStringProperty(ProjectProperty PropertyId, int lIndex);

		public static string aaApi_GetProjectStringProperty(ProjectProperty PropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetProjectStringProperty(PropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetProjectNumericProperty(ProjectProperty PropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetProjectGuidProperty")]
		private static extern IntPtr unsafe_aaApi_GetProjectGuidProperty(ProjectProperty PropertyId, int lIndex);

		public static Guid aaApi_GetProjectGuidProperty(ProjectProperty PropertyId, int Index)
		{
			IntPtr ptr = unsafe_aaApi_GetProjectGuidProperty(PropertyId, Index);
			return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetProjectNamePath2(int ProjectId, bool UseDesc, char tchSeparator, StringBuilder StringBuffer, int BufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentFileName(int ProjectId, int lDocumentId, StringBuilder StringBuffer, int BufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentFileSize64(int ProjectId, int lDocumentId, ref IntPtr pFileSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocument(int ProjectId, int lDocumentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GUIDSelectDocument(ref Guid documentGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GUIDSelectDocumentDataBuffer(ref Guid docGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDocumentNumericProperty(DocumentProperty PropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern ulong aaApi_GetDocumentUint64Property(DocumentProperty PropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDocumentStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetDocumentStringProperty(DocumentProperty PropertyId, int Index);

		public static string aaApi_GetDocumentStringProperty(DocumentProperty PropertyId, int Index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetDocumentStringProperty(PropertyId, Index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDocumentGuidProperty")]
		private static extern IntPtr unsafe_aaApi_GetDocumentGuidProperty(DocumentProperty PropertyId, int Index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ChangeDocumentVersion(NewVersionCreationFlags ulFlags, int vaultID, int documentID, int documentverID, string version, string comment, ref int versionDocId);

		public static Guid aaApi_GetDocumentGuidProperty(DocumentProperty PropertyId, int Index)
		{
			IntPtr ptr = unsafe_aaApi_GetDocumentGuidProperty(PropertyId, Index);
			return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDocumentCount(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_IsCurrentUserAdmin();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_NewDocumentVersion(NewVersionCreationFlags ulFlags, int vaultID, int documentID, string version, string comment, ref int versionDocId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ChangeDocumentFile(int vaultID, int documentID, string fileName);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ChangeDocumentFile2(int vaultID, int documentID, string fileName, DocumentFileFlags ulFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CopyOutDocument(int lProjectNo, int lDocumentId, string lpctstrWorkdir, StringBuilder lptstrFileName, int lBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDCopyOutDocument(ref Guid documentGuid, string lpctstrWorkdir, StringBuilder lptstrFileName, int lBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDFetchDocumentFromServer(DocFetchFlags ulFlags, ref Guid documentGuid, string workingDirectory, StringBuilder lptstrFileName, int lBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateLSet(ref int lpSetId, int lParentProjectId, int lParentDocumentId, int lChildProjectId, int lChildDocumentId, SetRelationType lRelationType, string lpctstrTransfer, ref int lpMemberId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AddLSetMember(int lSetId, int lChildProjectId, int lChildDocumentId, SetRelationType lRelationType, string lpctstrTransfer, ref int lpMemberId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDistinctSetIds(TypeMask ulTypeMask, int lParentProjectId, int lParentDocumentId, int lChildProjectId, int lChildDocumentID, ref IntPtr plplSetIds);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectSetReferences(int lParentProjectId, int lParentDocumentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectSetMasters(int lChildProjectId, int lChildDocumentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectSet(int lSetId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetSetNumericProperty(SetProperty lPropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvCount(bool bCountSystem);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvId(int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetCommonEnvId();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvNumericProperty(int lPropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetEnvStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetEnvStringProperty(int lPropertyId, int lIndex);

		public static string aaApi_GetEnvStringProperty(int lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetEnvStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAllEnvs(bool showSystem);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnvByProjectId(int projectID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetEnvStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetEnvStringProperty(EnvironmentProperty propertyID, int index);

		public static string aaApi_GetEnvStringProperty(EnvironmentProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetEnvStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvNumericProperty(EnvironmentProperty propertyID, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetActiveInterface();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAllGuis();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetActiveInterface(int interfaceID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetGuiStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetGuiStringProperty(InterfaceProperty propertyID, int index);

		public static string aaApi_GetGuiStringProperty(InterfaceProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetGuiStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetGuiId(int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnvAttrGuiDefs(int environmentID, int tableID, int columnID, int GuiID, int pageNo);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetEnvAttrGuiDefStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetEnvAttrGuiDefStringProperty(EnvAttributeGUIProperty propertyID, int index);

		public static string aaApi_GetEnvAttrGuiDefStringProperty(EnvAttributeGUIProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetEnvAttrGuiDefStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvAttrGuiDefNumericProperty(EnvAttributeGUIProperty propertyID, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_EnvAttrHolderCreate();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderDestroy(IntPtr hAttrHolder);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderInitWithNew(IntPtr hAttrHolder, int lTableId, int lProjectId, int lDocumentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderInitWithExisting(IntPtr hAttrHolder, int lTableId, int lAttrRecId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_EnvAttrHolderGetColumnCount(IntPtr hAttrHolder);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_EnvAttrHolderGetColumnId(IntPtr hAttrHolder, uint ulColumnIdx);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern string aaApi_EnvAttrHolderGetColumnName(IntPtr hAttrHolder, uint ulColumnIdx);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern string aaApi_EnvAttrHolderGetValue(IntPtr hAttrHolder, uint ulColumnIdx);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderSetValue(IntPtr hAttrHolder, uint ulColumnIdx, string lpctstrValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderGetValueById(IntPtr hAttrHolder, int lColumnId, ref string plpctstrValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderSetValueById(IntPtr hAttrHolder, int ulColumnIdx, string lpctstrValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderGetValueByName(IntPtr hAttrHolder, string columnName, out string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderSetValueByName(IntPtr hAttrHolder, string columnName, string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderGetColumnFlagsById(IntPtr hAttrHolder, int lColumnId, ref uint pFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderGetColumnFlagsByName(IntPtr hAttrHolder, string columnName, ref uint pFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderLoadDefaultFieldValue(IntPtr hAttrHolder, int defaultValueType, string defaultValueSource, string valueBuffer, uint bufferLength);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderUpdateTriggeredFields(IntPtr hAttrHolder, int lColumnId, ref int lplUpdatedColCount, ref int ppUpdatedColIds);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnvAttrHolderSetActiveHolder(IntPtr hAttrHolder);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_EnvAttrHolderGetActiveHolder();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDEnvAttrLoadDefaultValueByParam(int environmentId, ref Guid pDocGuid, int defValType, string defValSource, string valueBuffer, int bufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDEnvAttrHolderInitWithNew(IntPtr attrHolderHandle, int tableId, ref Guid pDocGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetSetStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetSetStringProperty(SetProperty lPropertyId, int lIndex);

		public static string aaApi_GetSetStringProperty(SetProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetSetStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateDocument(ref int documentID, int vaultID, int storageID, int fileType, DocumentType itemType, int applicationID, int departmentID, int workspaceProfileID, string sourceFilePath, string fileName, string name, string description, string version, bool leaveCheckedOut, DocumentCreationFlag creationFlags, StringBuilder workingFile, int workingFileBufferSize, ref int attributeID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDCreateDocument(out Guid documentGuid, ref Guid projectId, int storageID, int fileType, DocumentType itemType, int applicationID, int departmentID, int workspaceProfileID, string sourceFilePath, string fileName, string name, string description, string version, bool leaveCheckedOut, DocumentCreationFlag creationFlags, StringBuilder workingFile, int workingFileBufferSize, ref int attributeID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDModifyDocument(ref Guid documentGuid, int fileType, int itemType, int applicationId, int departmentId, int workspaceProfileId, string docFileName, string docName, string docDesc);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_MoveDocument(int lSourceProjectNo, int lSourceDocumentId, int lTargetProjectNo, ref int lpTargetDocumentId, string lpctstrWorkdir, string lpctstrFileName, string lpctstrName, string lpctstrDesc, DocumentCopyFlags ulFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CopyDocument(int lSourceProjectNo, int lSourceDocumentId, int lTargetProjectNo, ref int lpTargetDocumentId, string lpctstrWorkdir, string lpctstrFileName, string lpctstrName, string lpctstrDesc, DocumentCopyFlags ulFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CheckOutDocument(int vaultID, int documentID, string checkOutDirectory, StringBuilder localCopyFileNameAndPath, int localCopyFileNameAndPathBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDCheckOutDocument(ref Guid documentGuid, string checkOutDirectory, StringBuilder localCopyFileNameAndPath, int localCopyFileNameAndPathBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDRefreshDocumentServerCopy(ref Guid documentGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDGetDocumentCheckedOutFileName(ref Guid documentGuid, StringBuilder localCopyFileNameAndPath, int localCopyFileNameAndPathBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SimpleSearchCreateCriteriaBuffers(string queryString, IntPtr predefinedCriteriaBuf, out IntPtr documentCriteriaBuf, out IntPtr projectCriteriaBuf);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CheckInDocument(int vaultID, int documentID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CheckInDocumentLeaveCopy(int vaultID, int documentID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDCheckInDocument(ref Guid documentGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDPushDocumentToServer(DocPushFlags flags, ref Guid documentGuid, string comment);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_IsDocumentCheckedOut(int vaultID, int documentID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDIsDocumentCheckedOut(ref Guid documentGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_IsDocumentCheckedOutToMe(int vaultID, int documentID, out bool outToMe);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_IsDocumentCheckedIn(int vaultID, int documentID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDFreeDocument(ref Guid documentGuid, int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDPurgeDocumentCopy(ref Guid documentGuid, int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocumentsByProjectId(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocuments(IntPtr lpSelectInfo, IntPtr fpCallback, int lUserData);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocuments2(IntPtr lpSelectInfo, IntPtr fpCallback, int lUserData);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GUIDSelectDocumentsByProjectId(ref Guid projectGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocumentsByNameProp(int vaultID, string fileName, string name, string description, string version);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDocumentsByProp(int vaultID, int storageID, int fileType, int documentType, int applicationID, int departmentID, string fileName, string name, string description, string version, int versionNumber, int creatorID, int updaterID, int lastUserID, string status, int workflowID, int stateID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDocumentId(int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectChildProjects(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GUIDSelectChildProjects(ref Guid guid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetProjectId(int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectProjectDataBufferChilds(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectProjectDataBufferByStruct(int projectId, ref VaultDescriptor vaultDescriptor);

		[DllImport("dmscli.dll", SetLastError = true)]
		public static extern int aaApi_GetActiveDatasourceType();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GetDatasourceByName(string name);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern void aaApi_DatasourceIsDisconnected(IntPtr hDataSource, ref bool isDisconnected);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetConnectionInfo(IntPtr hDataSource, ref bool lpbODBC, ref int lplNativeType, ref int lplLoginType, StringBuilder lptstrName, int lLenName, StringBuilder lptstrUser, int lLenUser, StringBuilder lptstrPassword, int lLenPassword, StringBuilder lptstrSchema, int lLenSchema);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetConnectionInfo2(IntPtr hDataSource, ref bool lpbODBC, ref int lplNativeType, ref int lplLoginType, StringBuilder lptstrName, int lLenName, StringBuilder lptstrUser, int lLenUser, StringBuilder lptstrSchema, int lLenSchema);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDatasourceType(int datasourceIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDatasourceInterfaceType(int datasourceIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDatasourceName")]
		private static extern IntPtr unsafe_aaApi_GetDatasourceName(int index);

		public static string aaApi_GetDatasourceName(int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetDatasourceName(index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDatasourceInternalName")]
		private static extern IntPtr unsafe_aaApi_GetDatasourceInternalName(int index);

		public static string aaApi_GetDatasourceInternalName(int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetDatasourceInternalName(index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDatasourceFullName")]
		private static extern IntPtr unsafe_aaApi_GetDatasourceFullName(int index);

		public static string aaApi_GetDatasourceFullName(int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetDatasourceFullName(index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetInternalDatasourceName(IntPtr hDataSource, StringBuilder DsName, uint ulBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GetOpenDsHandles(ref int numOpenHandles);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetActiveDatasourceNativeType();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetActiveDatasourceName(StringBuilder lptstrName, int lNameSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GetActiveDatasource();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetCurrentSession(IntPtr handle);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetCurrentSession2(ulong handle);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetCurrentSession(ref IntPtr handle);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetCurrentSession2(ref ulong handle);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_ActivateDatasourceByHandle(IntPtr handle);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Login(dmawin.DataSourceType lDSType, string lptstrDataSource, string lpctstrUsername, string lpctstrPassword, string lpctstrSchema);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Login2(dmawin.DataSourceType lDSType, string lptstrDataSource, string lpctstrUsername, string lpctstrPassword, string lpctstrSchema, string lpctstrHostName, ulong productId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AdminLogin(dmawin.DataSourceType lDSType, string lpctstrDSource, string lpctstrUser, string lpctstrPassword);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AdminLogin2(dmawin.DataSourceType lDSType, string lpctstrDSource, string lpctstrUser, string lpctstrPassword, string lpctstrHostName, ulong productId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern void aaApi_RegisterReconnectCallback(AAPROC_RECONNECT fn);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern void aaApi_InvalidateConnectionCache();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Initialize(int init);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Uninitialize();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_Logout(string lptstrDataSource);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_LogoutByHandle(IntPtr hDSource);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDatasources();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDatasourcesByServer(int ulFlags, int ulServerAddr, dmawin.DataSourceType dstype);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDsServers();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDsServerNumericProperty(int lPropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_RefreshDatasourceList();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnv(int lEnvironmentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectApplication(int lApplId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetApplicationStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetApplicationStringProperty(ApplicationProperty lPropertyId, int lIndex);

		public static string aaApi_GetApplicationStringProperty(ApplicationProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetApplicationStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateUser(ref int userId, string userType, string secProvider, string name, string password, string description, string email);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ModifyUserExt(int userId, string userType, string secProvider, string name, string password, string description, string email);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteUserById(int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectUser(int lUserId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectUsersByProp(string name, string description, string email);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectUsersByGroup(int groupId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_AddUserToGroup(int groupId, int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_RemoveUserFromGroup(int groupId, int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetUserId(int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetCurrentUserId();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_VerifyUser(string userName, string password);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetUserNumericProperty(UserProperty lPropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetUserStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetUserStringProperty(UserProperty lPropertyId, int lIndex);

		public static string aaApi_GetUserStringProperty(UserProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetUserStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetUserStringSetting(int param, StringBuilder stringBuffer, int bufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_EnableUserAccount(int userId, bool enable);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateGroup(out int lplGroupId, string lpctstrType, string lpctstrSecProvider, string lpctstrName, string lpctstrDesc);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteGroupById(int lGroupId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectGroupDataBuffer();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectGroupDataBufferById(int lGroupId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectGroupDataBufferByUser(int userId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectDepartment(int lDepartmentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDepartmentStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetDepartmentStringProperty(DepartmentProperty lPropertyId, int lIndex);

		public static string aaApi_GetDepartmentStringProperty(DepartmentProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetDepartmentStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectState(int lStateId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetStateStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetStateStringProperty(StateProperty lPropertyId, int lIndex);

		public static string aaApi_GetStateStringProperty(StateProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetStateStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectAllStorages();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectStorage(int lStorageId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetStorageNumericProperty(StorageProperty lPropertyId, int lIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetStorageStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetStorageStringProperty(StorageProperty lPropertyId, int lIndex);

		public static string aaApi_GetStorageStringProperty(StorageProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetStorageStringProperty(lPropertyId, lIndex));
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct AASTORAGE_ATTR
		{
			public ulong cbSize;
			public ulong ullUserFreeSpace;
			public ulong ullUserVolumeSize;
			public ulong ullTotalVolumeSize;
			public ulong ullTotalFileSize;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetStorageAttributes")]
		public static extern bool unsafe_aaApi_GetStorageAttributes(int storageId, IntPtr lpStorageAttr);
		
		public static AASTORAGE_ATTR aaApi_GetStorageAttributes(int storageId)
        {
			var attr = new AASTORAGE_ATTR();
			attr.cbSize = (ulong)Marshal.SizeOf(attr);
			IntPtr pd = Util.StructureToPtr<AASTORAGE_ATTR>(attr);
			var ret = unsafe_aaApi_GetStorageAttributes(storageId, pd);
            if (!ret)
            {
				Marshal.FreeHGlobal(pd);
				throw PWException.GetPWLastException();
			}
			// 此方法里已经将指针free掉了
			attr = Util.PtrToStructure<AASTORAGE_ATTR>(pd);
			return attr;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectWorkflow(int lWorkflowId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetWorkflowStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetWorkflowStringProperty(WorkflowProperty lPropertyId, int lIndex);

		public static string aaApi_GetWorkflowStringProperty(WorkflowProperty lPropertyId, int lIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetWorkflowStringProperty(lPropertyId, lIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectLinkDataByObject(int lTableId, ObjectTypeForLinkData lItemType, int lItemId1, int lItemId2, string lpctstrWhere, ref int lplColumnCount, int[] lplColumnIds, LinkDataSelectFlags ulFlags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetLinkDataColumnCount();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetLinkDataNumericColumnValue(int rowIndex, int columnIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetLinkDataColumnValue")]
		private static extern IntPtr unsafe_aaApi_GetLinkDataColumnValue(int lRowIndex, int lColumnIndex);

		public static string aaApi_GetLinkDataColumnValue(int lRowIndex, int lColumnIndex)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetLinkDataColumnValue(lRowIndex, lColumnIndex));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateLink(int vaultID, int documentID, int tableID, int columnID, string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateLinkDataAndLink(int tableID, AttributeLinkageType linkType, int objectID1, int objectID2, ref int columnID, StringBuilder columnValue, int columnValueBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetLinkDataColumnValue(int tableID, int columnID, string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_FreeLinkDataInsertDesc();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectLinks(int vaultID, int documentID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_FreeLinkDataUpdateDesc();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_UpdateLinkDataColumnValue(int tableID, int columnID, string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_UpdateLinkData(int tableID, int columnID, string columnValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetLinkNumericProperty(LinkProperty propertyID, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetLinkStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetLinkStringProperty(LinkProperty propertyID, int index);

		public static string aaApi_GetLinkStringProperty(LinkProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetLinkStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetLinkDataColumnStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetLinkDataColumnStringProperty(LinkDataProperty propertyID, int index);

		public static string aaApi_GetLinkDataColumnStringProperty(LinkDataProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetLinkDataColumnStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectLinkData(int tableID, int columnID, string columnValue, ref int numColumns);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetDistinctIdentifiers(int lItemType, int lItemId1, int lItemId2, ref IntPtr lppIdentifiers);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteDocument(DocumentDeleteMasks DeleteFlags, int ProjectId, int DocumentId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GUIDDeleteDocument(DocumentDeleteMasks DeleteFlags, ref Guid documentGuid);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteProjectById(int ProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_DmsDataBufferSelect(int DsmBufferType);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_DmsDataBufferGetCount(IntPtr hDataBuffer);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_DmsDataBufferGetNumericProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_DmsDataBufferGetStringProperty")]
		private static extern IntPtr unsafe_aaApi_DmsDataBufferGetStringProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow);

		public static string aaApi_DmsDataBufferGetStringProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_DmsDataBufferGetStringProperty(hDataBuffer, lPropertyId, lIdxRow));
		}

		public static Guid aaApi_DmsDataBufferGetGuidProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow)
		{
			IntPtr ptr = unsafe_aaApi_DmsDataBufferGetGuidProperty(hDataBuffer, lPropertyId, lIdxRow);
			return (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_DmsDataBufferGetGuidProperty")]
		private static extern IntPtr unsafe_aaApi_DmsDataBufferGetGuidProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_DmsDataBufferGetBinaryProperty(IntPtr hDataBuffer, int lPropertyId, int lIdxRow);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern void aaApi_DmsDataBufferFree(IntPtr hDataBuffer);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_DmsDataBufferCreateEmpty(int bufferType);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_DmsThreadBufferGetStringProperty")]
		private static extern IntPtr unsafe_aaApi_DmsThreadBufferGetStringProperty(int lBufferId, int lPropertyId, int lIdxRow);

		public static string aaApi_DmsThreadBufferGetStringProperty(int lBufferId, int lPropertyId, int lIdxRow)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_DmsThreadBufferGetStringProperty(lBufferId, lPropertyId, lIdxRow));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnvCodeDefs(int environmentID, int tableID, int columnID, CodeDefinitionType type);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvCodeDefNumericProperty(DocumentCodeDefinitionProperty propertyID, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnvAttrDefs(int environmentID, int tableID, int columnID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetEnvAttrDefNumericProperty(AttributeDefinitionProperty property, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetEnvAttrDefStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetEnvAttrDefStringProperty(AttributeDefinitionProperty property, int index);

		public static string aaApi_GetEnvAttrDefStringProperty(AttributeDefinitionProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetEnvAttrDefStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectEnvByTableId(int tableID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateEnvAttr(int tableID, ref EnvironmentAttributeDocumentLinkage linkage, out int attributeRecordId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_UpdateEnvAttr(int tableID, int attributeRecordID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectColumn(int tableID, int columnID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetColumnNumericProperty(ColumnProperty property, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetColumnStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetColumnStringProperty(ColumnProperty property, int index);

		public static string aaApi_GetColumnStringProperty(ColumnProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetColumnStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetLinkDataColumnNumericProperty(LinkDataProperty property, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SqlSelect(string sqlStatement, IntPtr columnBind, ref int numColumnsSelected);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SqlSelectGetNumericProperty(SqlSelectProperty property, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_SqlSelectGetStringProperty")]
		private static extern IntPtr unsafe_aaApi_SqlSelectGetStringProperty(SqlSelectProperty property, int index);

		public static string aaApi_SqlSelectGetStringProperty(SqlSelectProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_SqlSelectGetStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SqlSelectDataBuffer(string sqlStatement, IntPtr columnBind);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SqlSelectDataBufGetNumericValue(IntPtr hDataBuffer, int row, int column, out int val);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SqlSelectDataBufGetDoubleValue(IntPtr hDataBuffer, int row, int column, out double val);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectTable(int tableID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetTableStringProperty")]
		private static extern IntPtr unsafe_aaApi_GetTableStringProperty(TableProperty property, int index);

		public static string aaApi_GetTableStringProperty(TableProperty propertyID, int index)
		{
			return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetTableStringProperty(propertyID, index));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SqlSelectDataBufGetValue(IntPtr hDataBuffer, int lIdxRow, int lIdxColumn);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetUserNumericSetting(int lParam);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetUserNumericSettingByUser(int lUserNo, int lParam);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SetUserNumericSetting(int lParam, int lParamValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_SaveUserSettings();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetFExtensionApplication(string fileExtension);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentGUIDsByIds([In] int lCount, [In] ref dmawin.AaDocItem pDocuments, [Out] Guid[] docGuids);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetProjectIdsByGUIDs([In] int lCount, [In] Guid[] docGuids, [Out] int[] plProjectIds);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetProjectGUIDsByIds([In] int lCount, [In] int[] plProjectIds, [Out] Guid[] docGuids);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SQueryDataBufferSelect(int lQueryId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CodeSerialGenerate(int environmentID, int serialTableID, int serialColumnID, FindSerialNumberRules[] serialGenerationRules, int[] attributeRecordNumbers, int attributeRecordCount, [Out] int[] generatedSerialNumbers);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CodeSerialGenerateInit();

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CodeSerialGenerateFieldAdd(int tableID, int columnID, string attributeValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_FetchDocumentFromServer(DocFetchFlags ulFlags, int lProjectId, int lDocumentId, string lpctstrWorkdir, StringBuilder lptstrFileName, int lBufferSize);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_FindItemsToBuffer")]
		internal static extern bool _aaApi_FindItemsToBuffer(IntPtr searchContext, IntPtr hCriteriaBuf, ref int pbCancel, ref IntPtr ppDataBuffer);

		public static bool aaApi_FindItemsToBuffer(SearchContext searchContext, IntPtr hCriteriaBuf, ref int pbCancel, ref IntPtr ppDataBuffer)
		{
			return _aaApi_FindItemsToBuffer(MarshalSearchContextStruct(searchContext), hCriteriaBuf, ref pbCancel, ref ppDataBuffer);
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_FindDocumentsToBuffer(IntPtr hCriteriaBuf, IntPtr lpResultColumns, ref int pbCancel, ref IntPtr ppDataBuffer);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SQueryCriDataBufferSelect(int lQueryId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SQueryCriDataBufferAddCriterion(IntPtr hCriteriaBuf, int lOrGroup, int lFlags, ref Guid lpcPropertySet, string lpctstrPropertyName, int lPropertyId, int lRelationId, int lFieldType, string lpctstrFieldValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SQueryCriDataBufferAddUnrestrictedLengthCriterion(IntPtr hCriteriaBuf, int lOrGroup, int lFlags, ref Guid lpcPropertySet, string lpctstrPropertyName, int lPropertyId, int lRelationId, int lFieldType, string lpctstrFieldValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectNestedReferencesList(int vaultID, int docID, int lParam, int nestDepth);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GUIDSelectNestedReferencesList(ref Guid guid, int lParam, int nestDepth);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GUIDSelectNestedReferencesList(ref Guid guid, NestedReferencesScanFlags ulFlag, int nestDepth);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GuidListGetSize(IntPtr guidListP);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GuidListGetAt(IntPtr guidListP, int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GuidListDestroy(IntPtr guidListP);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentIdsByGUIDs(int lItemCount, IntPtr pDocGuids, ref dmawin.AaDocItem pDocIds);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentIdsByGUIDs(int lItemCount, ref Guid pDocGuids, ref dmawin.AaDocItem pDocIds);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentGUIDsByIds(int lItemCount, ref dmawin.AaDocItem pDocIds, ref Guid pDocGuids);

		[DllImport("dmscli.dll")]
		public static extern int aaApi_SelectColumnsByTable(int tableID);

		[DllImport("dmscli.dll")]
		public static extern int aaApi_GetColumnId(int index);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_ModifyDocument(int vaultID, int docID, int modifiedFileType, int modifiedItemType, int modifiedApplicationID, int modifiedDepartmentID, int modifiedWorkspaceProfileID, string modifiedFileName, string modifiedName, string modifiedDescription);

		[DllImport("dmscli.dll")]
		public static extern IntPtr aaApi_SelectDataSourceDataBufferByHandle(IntPtr hDatasource);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectReferenceInformation(ulong refElemID, ref Guid pMasterGuid, int masterModelId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetReferenceInformationNumericProperty(IntPtr hDataBuffer, ReferenceInfoProperty propertyID, int rowIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_CreateReferenceInformation(ulong refElemID, ref Guid pMasterGuid, int masterModelId, ref Guid pReferenceGuid, int referenceModelID, int referenceType, int nestDepth, int flags);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_GetReferenceInformationGuidProperty(IntPtr hDataBuffer, ReferenceInfoProperty propertyID, int rowIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern ulong aaApi_GetReferenceInformationUint64Property(IntPtr hDataBuffer, ReferenceInfoProperty propertyID, int rowIndex);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_GetReferenceInformationCount(IntPtr hDataBuffer);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_DeleteReferenceInformation(ulong refElemID, ref Guid pMasterGuid, int masterModelID);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_DeleteSet(int lProjectId, int lDocumentId, int lSetId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectProjectsByProp(string lpctstrCode, string lpctstrName, string lpctstrDesc, string lpctstrVersion);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern int aaApi_SelectParentProject(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectProjectDataBuffer(int lProjectId);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectProjectDataBufferByProp(string code, string name, string description, string version);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CreateAuditTrailRecordByGUID(AuditTrailTypes lObjectTypeId, ref Guid lpcguidObjGUID, AuditTrailActions lActionTypeId, string lpctstrComment, int lParam1, int lParam2, string lpctstrParam, ref Guid lpcguidGUIDParam);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaApi_SelectAuditTrailRecords(IntPtr hCriteriaBuf);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GenSettingGetStringValue(ref Guid pNamespaceGuid, int settingId, StringBuilder strValue, ref int pBufferLen);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GenSettingSetStringValue(ref Guid pNamespaceGuid, int settingId, string strValue);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CheckDocumentAgainstQueryCriteria(int projectId, int documentId, IntPtr hQueryCriteria, ref bool pSatisfies);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_CheckDocumentGuidAgainstQueryCriteria(ref Guid pDocGuid, IntPtr hQueryCriteria, ref bool pSatisfies);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentMonikers(int lItemCount, dmawin.AaDocItem[] pDocIds, IntPtr[] pMonikers);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetDocumentMonikersByGuids(int lItemCount, Guid[] documentGuids, IntPtr[] pMonikers);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_StringsToMonikers")]
		private static extern bool unsafe_StringsToMonikers(int count, IntPtr[] monikers, IntPtr[] strings, MonikerFlags flags);

		public static bool aaApi_StringsToMonikers(int count, IntPtr[] monikers, string[] strings, MonikerFlags flags)
		{
			IntPtr[] array = new IntPtr[count];
			for (int i = 0; i < count; i++)
			{
				ref IntPtr reference = ref array[i];
				reference = Marshal.StringToCoTaskMemUni(strings[i]);
			}
			bool result = unsafe_StringsToMonikers(count, monikers, array, flags);
			IntPtr[] array2 = array;
			foreach (IntPtr ptr in array2)
			{
				Marshal.FreeCoTaskMem(ptr);
			}
			return result;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_MonikersToStrings")]
		private static extern bool unsafe_aaApi_MonikersToStrings(int count, IntPtr[] monikers, ref IntPtr strings, MonikerFlags flags);

		public static bool aaApi_MonikersToStrings(int count, IntPtr[] monikers, ref string[] strings, MonikerFlags flags)
		{
			IntPtr strings2 = IntPtr.Zero;
			if (!unsafe_aaApi_MonikersToStrings(count, monikers, ref strings2, flags))
			{
				return false;
			}
			string[] array = new string[count];
			IntPtr[] array2 = new IntPtr[count];
			Marshal.Copy(strings2, array2, 0, count);
			for (int i = 0; i < count; i++)
			{
				array[i] = Util.ConvertIntPtrToStringUnicode(array2[i]);
				dmsgen.aaApi_Free(array2[i]);
			}
			dmsgen.aaApi_Free(strings2);
			strings = array;
			return true;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_GetProjectMonikers(int lItemCount, int[] pProjectIds, IntPtr[] pMonikers);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetProjectGuidFromMoniker")]
		private static extern IntPtr unsafe_GetProjectGuidFromMoniker(IntPtr moniker);

		public static Guid aaApi_GetProjectGuidFromMoniker(IntPtr moniker)
		{
			IntPtr intPtr = unsafe_GetProjectGuidFromMoniker(moniker);
			if (IntPtr.Zero == intPtr)
			{
				return Guid.Empty;
			}
			return (Guid)Marshal.PtrToStructure(intPtr, typeof(Guid));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetDocumentGuidFromMoniker")]
		private static extern IntPtr unsafe_GetDocumentGuidFromMoniker(IntPtr moniker);

		public static Guid aaApi_GetDocumentGuidFromMoniker(IntPtr moniker)
		{
			IntPtr intPtr = unsafe_GetDocumentGuidFromMoniker(moniker);
			if (IntPtr.Zero == intPtr)
			{
				return Guid.Empty;
			}
			return (Guid)Marshal.PtrToStructure(intPtr, typeof(Guid));
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaApi_IsTrustedClient(string hostName, ref bool isTrusted);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_EncryptStringByPwd")]
		private static extern IntPtr unsafe_aaApi_EncryptStringByPwd(string inputString, string password);

		public static string aaApi_EncryptStringByPwd(string inputString, string password)
		{
			IntPtr intPtr = unsafe_aaApi_EncryptStringByPwd(inputString, password);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			string result = Util.ConvertIntPtrToStringUnicode(intPtr);
			dmsgen.aaApi_Free(intPtr);
			return result;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_DecryptStringByPwd")]
		private static extern IntPtr unsafe_aaApi_DecryptStringByPwd(string inputString, string password);

		public static string aaApi_DecryptStringByPwd(string inputString, string password)
		{
			IntPtr intPtr = unsafe_aaApi_DecryptStringByPwd(inputString, password);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			string result = Util.ConvertIntPtrToStringUnicode(intPtr);
			dmsgen.aaApi_Free(intPtr);
			return result;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetUserPassword")]
		private static extern IntPtr unsafe_aaApi_GetUserPassword(string szUser);

		public static string aaApi_GetUserPassword(string szUser)
		{
			IntPtr intPtr = unsafe_aaApi_GetUserPassword(szUser);
			if (intPtr == IntPtr.Zero)
			{
				return null;
			}
			string result = Util.ConvertIntPtrToStringUnicode(intPtr);
			dmsgen.aaApi_Free(intPtr);
			return result;
		}

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_FindSRS(out Guid srsGuid, string srsName);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_SelectObjectsLocations(ref IntPtr pSpatialObjLocs, ref IntPtr pSpatialLocations, SpatialDmsObject[] pSpatialObjects, int spatialObjectsCount, ref Guid pGuidSRS);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr aaSpatial_SelectObjectLocation(ref SpatialDmsObject pSpatialObject);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_CreateGeometryPoint(ref WKPoint3d point, ref IntPtr ppGeom);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_CreateGeometryLine(WKPoint3d[] points, int pointCount, ref IntPtr ppGeom);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_CreateGeometryPolygon(WKPoint3d[] points, int pointCount, ref IntPtr ppGeom);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_SetLocation(int objectType, ref Guid objectGuid, int accessRights, ref Guid srsGuid, IntPtr pGeom);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_ConvertBinaryToText(IntPtr pSrcGeometry, StringBuilder pDstGeometry, int destChars);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryType(IntPtr pGeom, ref int pType);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryPoint([In] IntPtr pGeom, out WKPoint3d pPoint);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryLinePointCount(IntPtr pGeom, ref int pPointCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryLinePoints([In] IntPtr pGeom, [In] int maxPoints, [In][Out] WKPoint3d[] points, [In][Out] ref int pPointCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryPolygonPointCount(IntPtr pGeom, ref int pPointCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryPolygonPoints([In] IntPtr pGeom, [In] int maxPoints, [In][Out] WKPoint3d[] points, [In][Out] ref int pPointCount);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_GetGeometryRange(IntPtr pGeom, out WKPoint3d pMin, out WKPoint3d pMax);

		[DllImport("dmscli.dll", CharSet = CharSet.Unicode)]
		public static extern bool aaSpatial_DestroyGeometry(ref IntPtr ppGeom);

		[DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
		public static extern ModuleFlags aaApi_GetModuleFlags();

		[DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
		public static extern void aaApi_SetModuleFlags(ModuleFlags flags);
	}

}
