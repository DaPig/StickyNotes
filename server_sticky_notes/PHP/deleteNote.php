<?php

$servername = ""; // server name
$server_username = ""; // username for server
$server_password = ""; // insert server password
$dbName = ""; // insert database name

		$id = $_POST["id"];


$conn = new mysqli($servername, $server_username, $server_password, $dbName);
		
		if(!$conn){
			die("connection failed.". mysqli_connect_error());
		}
	
		$sql = "DELETE FROM note WHERE id = $id";
		$result = mysqli_query($conn, $sql);
		
		if(!result) echo "there was an error";
		else echo "Everything ok.";
		
?>