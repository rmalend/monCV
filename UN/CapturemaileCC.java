  Document buildMailDocument(Document doc) {
    final String TAG_SUBJECT ="<Subject>"; 
    final String TAG_CASENUM ="<CaseNumber>"; 
    final String TAG_OFFICER ="<ProcurementOfficer>";
    final String TAG_CREATOR = "<CreatedBy>";
    final String TAG_PDBUYER = "<PDBuyer>";
    final String TAG_OWNER = "<Owner>";
    final String TAG_URL  = "<DocURL>";
    final String TAG_MISSION = "<Mission>";
    final String TAG_COMMT = "<MemberComments>";
    final String TAG_LOANUM ="<LOANumber>"; 
	try {
      Document memo = null;
      String httpLink = "http://", mailHTML="";  
      Vector<String> vBcc = processDoc.getItemValue("Mail_BlindCopyTo");
      Vector vCc = processDoc.getItemValue("Mail_CopyTo");
      Vector vTo = processDoc.getItemValue("Mail_SendTo");
      String subject = processDoc.getItemValueString("Mail_Subject");
      String dbURL = profileDoc.getItemValueString("Prof_dbURL");
      String httpname= profileDoc.getItemValueString("Prof_HTTPName");
      String mailFormat= profileDoc.getItemValueString("Prof_ContentHeader");
            
      if ( subject.trim().isEmpty()) return memo;         
      memo = eccdb.createDocument();
      memo.replaceItemValue("Form", "Memo");
      memo.replaceItemValue("Principal", processDoc.getItemValueString("Mail_Principal"));
      String element="";
      Vector vect1 = new Vector();   
      Vector vect2 = new Vector();
      Vector vect3 = new Vector();
      for (int i=0; i<vTo.size(); i++){
          element = replaceVariableTagArray( (String)vTo.elementAt(i),"", doc);
          vect1.addElement( element) ;
      }

      for (int i=0; i<vCc.size(); i++){
                 element = replaceVariableTagArray( (String)vCc.elementAt(i),"", doc);
                 vect2.addElement( element) ;
      }

      for (int i=0; i<vBcc.size(); i++){
                 element = replaceVariableTagArray( (String)vBcc.elementAt(i),"", doc);
                 vect3.addElement( element) ;
      }
      
      memo.appendItemValue("SendTo", vect1);
      memo.appendItemValue("CopyTo", vect2);
      memo.appendItemValue("BlindCopyTo", vect3);
      if (!copyOOO.isEmpty())
           memo.appendItemValue("CopyTo", copyOOO.elementAt(0));
  
      if ( mailFormat.equals("HTML") ){
          httpLink = "<a href='http://" + httpname+"/" +dbURL;
          httpLink = httpLink + "/$$OpenDominoDocument.xsp?documentId="+doc.getUniversalID()+ "&action=openDocument'> ";
          httpLink = httpLink + "Link to eCC Case</a>";
 	
      }else {
       httpLink = httpLink + httpname+"/" +dbURL + "/$$OpenDominoDocument.xsp?documentId="+doc.getUniversalID()+ "&action=openDocument";
      }

      if ( mailFormat.equals("NOTES") ){
//         httpLink = httpLink + httpname+"/" +dbURL + "/$$OpenDominoDocument.xsp?documentId="+doc.getUniversalID()+ "&action=openDocument";
         RichTextItem processBody = (RichTextItem)processDoc.getFirstItem("Mail_Body");
      	 processBody.copyItemToDocument(memo,"Body");           
         RichTextItem rti = (RichTextItem)memo.getFirstItem("Body");
         RichTextNavigator rtnav = rti.createNavigator();
         if (rtnav.findFirstElement(RichTextItem.RTELEM_TYPE_TEXTPARAGRAPH)) { 
            RichTextRange rtrange = rti.createRange(); 
            long options= RichTextItem.RT_FIND_CASEINSENSITIVE +RichTextItem.RT_REPL_ALL +RichTextItem.RT_REPL_PRESERVECASE;            
            rtrange.findandReplace(TAG_SUBJECT ,  doc.getItemValueString("Subject") ,options );   
            rtrange.findandReplace(TAG_CASENUM,  doc.getItemValueString("CaseNumber") ,options );  
            rtrange.findandReplace(TAG_OFFICER,  doc.getItemValueString("CreatedBy") ,options );  
            rtrange.findandReplace(TAG_CREATOR,  doc.getItemValueString("CreatedBy") ,options );  
            rtrange.findandReplace(TAG_PDBUYER,  doc.getItemValueString("PDBuyer") ,options );  
            rtrange.findandReplace(TAG_OWNER,  doc.getItemValueString("Owner") ,options );
            rtrange.findandReplace(TAG_URL,  httpLink ,options );
            rtrange.findandReplace(TAG_MISSION,  paramDoc.getItemValueString("pAppName") ,options );  
            rtrange.findandReplace(TAG_COMMT,  doc.getItemValueString("MemberComments") ,options );  
            rtrange.findandReplace(TAG_LOANUM,  doc.getItemValueString("LOANumber") ,options );       
         }

       }else { // non NOTES format
           String mailBody = processDoc.getItemValueString("Mail_Body");
   //        httpLink = httpLink + httpname+"/" +dbURL + "/$$OpenDominoDocument.xsp?documentId="+doc.getUniversalID()+ "&action=openDocument";

           subject = replaceVariableTagArray(subject, "", doc); 
           mailBody = replaceVariableTagArray(mailBody, httpLink, doc);
           // ----------------------
           Stream stream = _sess.createStream();
           _sess.setConvertMIME(false); // Do not convert MIME to rich text

           MIMEEntity body = memo.createMIMEEntity(); // Create parent entity
           MIMEHeader header = body.createHeader("Content-Type");
           header.setHeaderVal("multipart/mixed");
           header = body.createHeader("Subject");
           header.setHeaderVal(subject);
           header = body.createHeader("Principal");
           header.setHeaderVal(processDoc.getItemValueString("Mail_Principal"));
           MIMEEntity child = body.createChildEntity(); // Create first child entity
//           child = body.createChildEntity();
           stream.writeText(mailBody);
           if ( mailFormat.equals("HTML") )
                child.setContentFromText(stream, "text/html", MIMEEntity.ENC_NONE);
           else child.setContentFromText(stream, "text/plain", MIMEEntity.ENC_NONE);

           stream.close();
            _sess.setConvertMIME(true); // Restore conversion

  
         //        mailBody = replaceVariableTagArray(mailBody, httpLink, doc);
       }	
      
       memo.setSaveMessageOnSend(false);
       memo.setSignOnSend(false);
       if ( recipients.isEmpty()){   // if extra emails (7000, 7001)
           memo.send(false );
       }
       else {                        // if regular emails with the workflow configuration
    	   for (int i=0; i<vTo.size(); i++){
             element = replaceVariableTagArray( (String)vTo.elementAt(i),"", doc);
             recipients.addElement( element) ;
    	   }       	
    	   memo.send(false, recipients);
       }          
   
  	      

     } catch (Exception e) {
       	    ecclog.logError(doc, " buildMailDocument() Exception ", e );
            e.printStackTrace();
      }
        return memo;
   }//
