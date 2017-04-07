<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";

		$username = $_POST["username"];
		$password = $_POST["password"];
		$email = $_POST["email"];

$conn = new mysqli($servername, $server_username, $server_password, $dbName);

		if(!$conn){
			die("connection failed.". mysqli_connect_error());
		}

		$sql = "INSERT INTO User (username, password, email) VALUES ('".$username."', '".$password."', '".$email."')";
		$result = mysqli_query($conn, $sql);

		if(!result) echo "there was an error";
		else echo "Everything ok.";

?>
