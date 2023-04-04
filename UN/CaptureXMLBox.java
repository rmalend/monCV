   
/* ------- String FillXMLBox( boxNum){
 * <rs-box>
	  <box-number>boxNum</box-number>
	  <rs-folder>
	  <folder-title>Support for primary education in Huambo &amp; Kuanza Sul provinces: School feeding programme - including HIV/AIDS awareness</folder-title>
	  <earliest-date>01/01/1999</earliest-date>
	  <latest-date>31/12/2007</latest-date>
	  <file-number>WFP-AF-05-001</file-number>
      </rs-folder>
   </rs-box> 
*/
 String FillXMLBox( String seriesNum, String boxNum){
   	 DocumentCollection foldersColl = null; 

	 String key = new String(seriesNum + "~~" + boxNum);
	 String ret = "";
	 String foldertitle ="";
 	 String nextvalue = "";
    
	 try {	 	 
		ret = "  <rs-box>\n";               
		ret = ret +  "   <box-number>" + boxNum + "</box-number>\n";
		foldersColl = viewFolder21.getAllDocumentsByKey( key, true);
        // build a list of UNIQUE folder title and sort the list alphabetically
        SortedSet tset = BuildOrderedListString(  foldersColl, "Fold_Title");	// need to sort out for TRIM
        if ( tset != null){
     	   Iterator iterator = tset.iterator();
     	   while (iterator.hasNext()) {              // for each boxNumber in the tset, get details
     		     nextvalue = iterator.next().toString();
    			 Document folder = foldersColl.getFirstDocument();
     	         while ( folder != null){
     	        	  foldertitle = folder.getItemValueString("Fold_Title");
     	        	  if ( nextvalue.equals(foldertitle) ){
     	        		  ret = ret + "   <rs-folder>\n";
     	        		  ret = ret + "    <folder-title>" + foldertitle +"</folder-title>\n";
     	        		  ret = ret + "    <earliest-date>" + folder.getItemValueString("Fold_EarliestDate")+ "</earliest-date>\n";
     	        		  ret = ret + "    <latest-date>" + folder.getItemValueString("Fold_LatestDate")+ "</latest-date>\n";
     	        		  ret = ret + "    <file-number>" + folder.getItemValueString("Fold_FileNumber")+ "</file-number>\n" ;
     	        		  ret = ret + "   </rs-folder>\n";
     	        	  }
     	        	  folder = foldersColl.getNextDocument( folder);	
     	         }

     	   }
        }		
/*		if ( foldersColl.getCount()>0 ){				
			 Document folder = foldersColl.getFirstDocument();
			 while ( folder != null){
				 ret = ret + "   <rs-folder>\n";
				 ret = ret + "    <folder-title>" + folder.getItemValueString("Fold_Title")+"</folder-title>\n";
				 ret = ret + "    <earliest-date>" + folder.getItemValueString("Fold_EarliestDate")+ "</earliest-date>\n";
				 ret = ret + "    <latest-date>" + folder.getItemValueString("Fold_LatestDate")+ "</latest-date>\n";
				 ret = ret + "    <file-number>" + folder.getItemValueString("Fold_FileNumber")+ "</file-number>\n" ;
				 ret = ret + "   </rs-folder>\n";
				 folder = foldersColl.getNextDocument( folder);	
			 }
		}
*/		ret = ret + "  </rs-box>\n";
		return ret;  			 

	  } catch (NotesException ex) {
	      ex.printStackTrace();
	  }
	return ret;  

 }
 