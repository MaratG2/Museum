<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

	$server = "fdb28.awardspace.net";
	$db_name = "4221494_museumistu";
	$db_username = "4221494_museumistu";
	$db_password = "t)s4+jV{91/ExlFt";
	
	$conn = mysqli_connect($server, $db_username, $db_password, $db_name);
    if ($conn)
    {
        echo 'Connection attempt succeeded.';
    }
    else
    {
        echo 'Connection attempt failed.';
    }
	
function OpenCon()
{
	$server = "fdb28.awardspace.net";
	$db_name = "4221494_museumistu";
	$db_username = "4221494_museumistu";
	$db_password = "t)s4+jV{91/ExlFt";
	$conn = new mysqli_connect($server, $db_username, $db_password, $db_name)
			or die("Connect attempt failed: %s\n". $conn -> error);
	
	if($conn)
	{
        echo 'Connection attempt succeeded.';
    }
	
	return $conn;
}
 
function CloseCon($conn)
{
	$conn -> close();
}
   
?>