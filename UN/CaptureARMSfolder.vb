Option Public
Option Declare
Use "SL_ReSe"
Sub Initialize
	
	' declare variables
	Dim ns As New NotesSession
	Dim ndb As NotesDatabase
	Dim nd_ReSe As NotesDocument
	Dim ni_UNIDs As NotesItem
	Dim nd_Fold As NotesDocument
	Dim v_status As Integer
	Dim v_url As String
	
	On Error Goto ErrHandler
	
	' set variables
	Set ndb = ns.CurrentDatabase
	Set nd_ReSe = ns.DocumentContext
	Set ni_UNIDs = nd_ReSe.GetFirstItem("totProcessdocs")
	v_status = Cint(nd_ReSe.status(0))
	
	' loop thru Folder docs, and change status (or delete)
	If v_status = 86 Then
		Forall v In ni_UNIDs.Values
			Set nd_Fold = ndb.GetDocumentByUNID(v)
			With nd_Fold
				.Form = "Deleted"
				Call .Save( True, False)
			End With
		End Forall
	Else
		Forall v In ni_UNIDs.Values
			Set nd_Fold = ndb.GetDocumentByUNID(v)
			With nd_Fold
				.Fold_StatusNum = v_status
				Call .Save( True, False)
			End With
		End Forall
	End If
	
	' update ReSe info
	With nd_ReSe
		.Fold_TotalsMsg = ReSe_UpdateFolders(nd_ReSe)
		.Save True, True
	End With
	
	' open on web
	v_url = "http://" & ns.InternetHostName & "." & ns.InternetDomainName & "/" & Replace(ndb.FilePath, "\", "/") & "/0/" & nd_ReSe.UniversalID
	Print |<SCRIPT language="JavaScript">|
	Print |window.location.href="| & v_url & |"|
	Print |</SCRIPT>|
	
	Exit Sub
	
ErrHandler:
	Call PrintErrorMsg("Code - " & Str(Err) & ": " & Error$ +". (onLine " & Str(Erl) & ") in sub " & Lsi_info(2) & ".", True)
	
End Sub
Sub PrintErrorMsg(errMsg As String, fatal As Integer)
	Print {
  <h3>An error has occured in the application.</h3>
  <h4>Please report the following message to the system administrator:</h4>
  <i>} & errMsg & {</i>
  <p/>
}
	
	If fatal Then End 
	
End Sub