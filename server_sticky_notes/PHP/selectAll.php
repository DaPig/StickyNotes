<?php

$servername = ""; // server name
$server_username = ""; // username for server
$server_password = ""; // insert server password
$dbName = ""; // insert database name


$conn = new mysqli($servername, $server_username, $server_password, $dbName);
		
		if(!$conn){
			die("connection failed.". mysqli_connect_error());
		}
	
		$sql = "SELECT * FROM  note";
		$result = mysqli_query($conn, $sql) or die("Error in Selecting " . mysqli_error($conn));
		
		$rows = array();
		while($row = mysqli_fetch_assoc($result))
		{
			$rows[] = $row;
		}
		
		$notearray = array('Notes' => $rows);
		echo json_encode($notearray);

?>