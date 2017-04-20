<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";


		$id = $_POST["id"];
		
		try {
		    $conn = new PDO("mysql:host=$servername;dbname=$dbName", $server_username, $server_password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		    echo "Connected successfully"; 
		    $sql = "DELETE FROM Notes WHERE id = $id";
		    $conn->exec($sql);
		    }
		catch(PDOException $e)
		    {
		    echo "Connection failed: " . $e->getMessage();
		    }
?>
?>