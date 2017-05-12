<?php

$servername = "libanaden.com.mysql";
$server_username = "libanaden_com_notes";
$server_password = "Dreamteam";
$dbName = "libanaden_com_notes";

		$note_id = $_POST["note_id"];
		$header_text = $_POST["header_text"];
		$position = $_POST["position"];

		try {
		    $conn = new PDO("mysql:host=$servername;dbname=$dbName", $server_username, $server_password);
		    // set the PDO error mode to exception
		    $conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
				$sql = "DELETE FROM consistsOf WHERE note_id = $note_id;
								UPDATE Notes SET position = $position WHERE id = $note_id;";
        $conn->exec($sql);
		    }
		catch(PDOException $e)
		    {
		    echo "Connection failed: " . $e->getMessage();
		    }

?>
