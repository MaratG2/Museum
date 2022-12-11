<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$conn = OpenCon();
$result = mysqli_query($conn, "SELECT * FROM halls ORDER BY hnum ASC");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	while ($row = $result->fetch_row())
	{
		echo ($row[0].'|'.$row[1].'|'.$row[2].'|'.$row[3].'|'.$row[4].'|'
		    .$row[5].'|'.$row[6].'|'.$row[7].'|'.$row[8].'|'.$row[9].'|'
			.$row[10] . ";");
	}
}
?>