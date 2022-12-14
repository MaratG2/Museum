<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$name = $_POST['name'];
$sizex = $_POST['sizex'];
$sizez = $_POST['sizez'];
$is_date_b = $_POST['is_date_b'];
$is_date_e = $_POST['is_date_e'];
$date_begin = $_POST['date_begin'];
$date_end = $_POST['date_end'];
$is_maintained = $_POST['is_maintained'];
$is_hidden = $_POST['is_hidden'];
$author = $_POST['author'];
$wall = $_POST['wall'];
$floor = $_POST['floor'];
$roof = $_POST['roof'];

$conn = OpenCon();
$result = mysqli_query($conn, 
"INSERT INTO halls (name, sizex, sizez, is_date_b, is_date_e, time_begin, time_end, is_maintained, is_hidden, author, wall, floor, roof)
 VALUES('".$name."',".$sizex.",".$sizez.",".$is_date_b.",".$is_date_e.",".$date_begin.",".$date_end.",".
 $is_maintained.",".$is_hidden.",'".$author."',".$wall.",".$floor.",".$roof.")");
 
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Inserted hall successfully: " . $name;
}
?>