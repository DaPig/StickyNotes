<?php


$servername = ""; // server name
$server_username = ""; // username for server
$server_password = ""; // insert server password
$dbName = ""; // insert database name


		$content =$_POST["content"];
		$user = $_POST["user"];

$conn = new mysqli($servername, $server_username, $server_password, $dbName);
		
		if(!$conn){
			die("connection failed.". mysqli_connect_error());
		}
	
		$sql = "INSERT INTO note (content) VALUES ('".$content."')";
		$result = mysqli_query($conn, $sql);
		//return mysqli_insert_id($conn);
		
		if(!result) echo "there was an error";
		else echo mysqli_insert_id($conn);
		return mysqli_insert_id($conn);
		
?>