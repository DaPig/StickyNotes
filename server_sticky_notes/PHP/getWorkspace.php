<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";

		$ws_id = $_POST["ws_id"];

		try {
		    $conn = new PDO("mysql:host=$servername;dbname=$dbName", $server_username, $server_password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
				$sql = "SELECT Workspace.id AS ws_id, Notes.id AS note_id, Notes.position, Notes.content, Headers.id AS header_id, Headers.position, Headers.header_text FROM Notes
				JOIN consistsOf ON Notes.id = consistsOf.note_id
				JOIN Workspace ON Workspace.id = consistsOf.ws_id
				JOIN Headers ON Headers.ws_id = Workspace.id
				WHERE Workspace.id = $ws_id";

				$stmt = $conn->prepare($sql);
      	$stmt->execute();

				$result = $stmt->fetchAll();
				echo $resultArray
		    }
		catch(PDOException $e)
		    {
		    echo "Connection failed: " . $e->getMessage();
		    }

?>
