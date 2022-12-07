<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_name = $_POST['name'];
$arg_email = $_POST['email'];
$arg_pass = $_POST['pass'];

$conn = OpenCon();
$result = mysqli_query($conn, "INSERT INTO users(name, email, password) VALUES('" . $arg_name . "', '" . $arg_email . "', '" . $arg_pass . "')");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Registered successfully: " . $arg_email;
}
?>