<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_hnum = $_POST['hnum'];
$arg_title = $_POST['title'];
$arg_image_url = $_POST['image_url'];
$arg_image_desc = $_POST['image_desc'];
$arg_combined_pos = $_POST['combined_pos'];
$arg_type = $_POST['type'];

$conn = OpenCon();
$result = mysqli_query($conn, 
"INSERT INTO contents (hnum, title, image_url, image_desc, combined_pos, type, operation)
 VALUES(".$arg_hnum.",'".$arg_title."','".$arg_image_url."','".$arg_image_desc."','".$arg_combined_pos."',".$arg_type.", 'INSERT')
 ON DUPLICATE KEY UPDATE 
	hnum = ".$arg_hnum.", 
	title = '".$arg_title."', 
	image_url = '".$arg_image_url."', 
	image_desc = '".$arg_image_desc."', 
	combined_pos = '".$arg_combined_pos."', 
	type = ".$arg_type.", 
	operation = 'UPDATE'");
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	echo "Query completed";
}
?>