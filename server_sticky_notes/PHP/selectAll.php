<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";

		try {
		    $conn = new PDO("mysql:host=$servername;dbname=$dbName", $server_username, $server_password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		    echo "Connected successfully"; 
		    $sql = "SELECT * FROM  Notes";
		    $conn->exec($sql);
		    $row = array();
		    while($row = PDO::FETCH_ASSOC){
		        $rows[] = $row;
		    }
		    $notearray = array('Notes' => $rows);
		    echo json_encode($notearray);
	    }
		catch(PDOException $e)
	    {
		    echo "Connection failed: " . $e->getMessage();
		}

?>