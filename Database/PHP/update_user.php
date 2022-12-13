<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_name = $_POST['name'];
$arg_email = $_POST['email'];
$arg_password = $_POST['password'];
$arg_access_level = $_POST['access_level'];

$conn = OpenCon();
$result = mysqli_query($conn, 
"UPDATE users SET name = '".$arg_name."', email = '".$arg_email."', password = '".$arg_password."', access_level = ".$arg_access_level."
 WHERE email = '".$arg_email."'");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Query completed";
}
?>