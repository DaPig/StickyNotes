<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";


		$content =$_POST["content"];
		$user = $_POST["user_id"];
		
		//else echo mysqli_insert_id($conn);
		//return mysqli_insert_id($conn);
		
		try {
		    $conn = new PDO("mysql:host=$servername;dbname=$dbName", $server_username, $server_password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		    $sql = "INSERT INTO Notes (content, user_id) VALUES ('".$content."', '".$user."')";
		    $conn->exec($sql);
		    $id = $conn->lastInsertId();
            echo $id; 
		    }
		catch(PDOException $e)
		    {
		    echo "Connection failed: " . $e->getMessage();
		    }
		
?>