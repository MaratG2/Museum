<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_hnum = $_POST['hnum'];

$conn = OpenCon();
$result = mysqli_query($conn, 
"SELECT * FROM halls AS h
 WHERE ".$arg_hnum." = h.hnum");
 
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	$row = $result->fetch_row();

	echo ($row[0].'|'.$row[1].'|'.$row[2].'|'.$row[3].'|'.$row[4].'|'
	    .$row[5].'|'.$row[6].'|'.$row[7].'|'.$row[8].'|'.$row[9].'|'
		.$row[10]);
}
?>