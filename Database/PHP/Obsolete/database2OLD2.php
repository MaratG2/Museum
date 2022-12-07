<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

	$host = "dpg-ce7op4h4rebbibivu030-a.frankfurt-postgres.render.com"; // Host name 
	$port = "5432"; //DB port
	$db_name = "museumistu_6g6y"; // DB name 
	$db_username = "maratg2"; // DB username 
	$db_password = "jVjNRxOEC2HxwANgA9WW4avOuA2zraiR"; // DB password 
	$db_sslmode = "prefer";
	$db_trust = "true";
	
	$db = pg_connect("host=" . $host . " port=" . $port . " dbname=" . $db_name . " user=" . $db_username . " password=" . $db_password . " sslmode=" . $db_sslmode . " Trust Server Certificate=" . $db_trust) or die("Connection error");
    if ($db)
    {
        echo 'Connection attempt succeeded.';
    }
    else
    {
        echo 'Connection attempt failed.';
    }
?>