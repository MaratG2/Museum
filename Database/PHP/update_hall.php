<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_name = $_POST['name'];
$arg_is_hidden = $_POST['is_hidden'];
$arg_is_maintained = $_POST['is_maintained'];
$arg_hnum = $_POST['hnum'];
$arg_wall = $_POST['wall'];
$arg_floor = $_POST['floor'];
$arg_roof = $_POST['roof'];
$is_date_b = $_POST['is_date_b'];
$is_date_e = $_POST['is_date_e'];
$time_begin = $_POST['time_begin'];
$time_end = $_POST['time_end'];

$conn = OpenCon();
$result = mysqli_query($conn, "UPDATE halls SET name = '".$arg_name."', is_hidden = ".$arg_is_hidden.", is_maintained = ".$arg_is_maintained.", wall = ".$arg_wall.", floor = ".$arg_floor.", roof = ".$arg_roof.", is_date_b = ".$is_date_b.", is_date_e = ".$is_date_e.", time_begin = '".$time_begin."', time_end = '".$time_end."' WHERE hnum = ".$arg_hnum);
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Query completed";
}
?>