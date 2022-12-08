<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_email = $_POST['email'];
$arg_pass = $_POST['pass'];

$conn = OpenCon();
$result = mysqli_query($conn, "SELECT * FROM users WHERE email = '" . $arg_email ."' AND password = '" . $arg_pass . "'");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	$row = $result->fetch_row();
	echo $row[0].'|'.$row[1].'|'.$row[2].'|'.$row[3].'|'.$row[4];
}
?>