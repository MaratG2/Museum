<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_hnum = $_POST['hnum'];
$arg_combined_pos = $_POST['combined_pos'];

$conn = OpenCon();
$result = mysqli_query($conn, "DELETE FROM contents WHERE combined_pos = '".$arg_combined_pos."' AND hnum = ".$arg_hnum);
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Query completed";
}
?>