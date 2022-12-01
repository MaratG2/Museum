<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

	$host = "ec2-54-77-40-202.eu-west-1.compute.amazonaws.com"; // Host name 
	$port = "5432"; //DB port
	$db_name = "dp3oh4vja8l35"; // DB name 
	$db_username = "eudqcffpovolpi"; // DB username 
	$db_password = "65f254f251471be22f035c26958c8cfad49fc31c9e8134febf4f4c165bd47665"; // DB password 

	$db = pg_connect("host=" . $host . " port=" . $port . " dbname=" . $db_name . " user=" . $db_username . "  password=" . $db_password . " sslmode=Prefer") or die("Connection error");
    if ($db)
    {
        echo 'Connection attempt succeeded.';
    }
    else
    {
        echo 'Connection attempt failed.';
    }
?>