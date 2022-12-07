<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_email = $_POST['email'];

$conn = OpenCon();
$result = mysqli_query($conn, "SELECT * FROM users WHERE email = '" . $arg_email ."'");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	$row_count = mysqli_num_rows($result);
	echo $row_count;
}
?>