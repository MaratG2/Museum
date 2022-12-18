<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

include 'database_connection.php';

$arg_hnum = $_POST['hnum'];

$conn = OpenCon();
$result = mysqli_query($conn, 
"SELECT c.cnum, c.title, c.image_url, c.image_desc, c.combined_pos, c.type, c.date_added, c.operation
 FROM contents AS c WHERE c.hnum = ".$arg_hnum);
if(!$result)
{
	echo "Query failed: " . $conn -> error;
}
else
{
	while ($row = $result->fetch_row())
	{
		echo ($row[0].'|'.$row[1].'|'.$row[2].'|'.$row[3].'|'.$row[4].'|'
		    .$row[5].'|'.$row[6].'|'.$row[7].";");
	}
}
?>