  private boolean initActivity() {

         try {
          _sess = getSession();
          agentContext = _sess.getAgentContext();
          eccdb = agentContext.getCurrentDatabase();
   		  viewParam = eccdb.getView("V_Admin");		
		  viewLEI = eccdb.getView("V_LEI");	
		  viewParam = eccdb.getView("V_Admin");			
		  paramDoc = viewParam.getFirstDocument();
		  if ( paramDoc.getItemValueString("pAppType").equals("local") ) return false;

		  profileDoc = eccdb.getProfileDocument("F_Prof", null);
		  SQLInstance  =  profileDoc.getItemValueString("Prof_SQLServer");
		  SQLDatabase  =  profileDoc.getItemValueString("Prof_SQLDatabase");
	      SQLUser =  profileDoc.getItemValueString("Prof_SQLogin");
	      SQLPwd  =  profileDoc.getItemValueString("Prof_SQLPwd");
			  
	  	  connectionUrl = ("jdbc:sqlserver://").concat(SQLInstance).concat(":1433;databaseName=").concat(SQLDatabase);
	   	  connectionUrl = connectionUrl.concat(";integratedSecurity=false;");
	      Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
	      String SQLTableName = SQLDatabase.concat(".dbo.ECC50DATA") ;
	      String  insertStr = "IF NOT EXISTS (SELECT * FROM " + SQLTableName+" WHERE CaseNumber=?  ) " 
	                	+  "\t INSERT INTO " + SQLTableName;	                
	      String columnsStr = " ([RequisitioningOfficer], [VendorName],[Broker],[MemberState], [AwardFinancialRule], [CaseNumber], [ExPostFacto], [FormType], [HQCaseMeeting], [HQCaseAgendaNo]," +
	                                   "  [HQMeetingDate], [LOANumber], [LOASection], [ProcurementFor], [Subject], [HQRecommendedAmount], [LOAAmount], [PartialEPFAmountUSD], [PartialNonEPFAmountUSD], [AwardAmount] ) ";
	      String valuesStr = " VALUES (?, ?, ?,?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";      	        
	      insertQuery = insertStr + columnsStr + valuesStr ;
	
		        	 
        } catch (Exception e) {
			e.printStackTrace();
		}
        return true;
	}