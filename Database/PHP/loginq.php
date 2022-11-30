<?php

header("Access-Control-Allow-Credentials: true");
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST, GET, OPTIONS');
header('Access-Control-Allow-Headers: Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time');

	if(isset($_POST["email"]))
	{
        $errors = array();

		$email = $_POST["email"];

        $host = "ec2-54-77-40-202.eu-west-1.compute.amazonaws.com"; // Host name
        $port = "5432"; //DB port
        $db_name = "dp3oh4vja8l35"; // DB name
        $db_username = "eudqcffpovolpi"; // DB username
        $db_password = "65f254f251471be22f035c26958c8cfad49fc31c9e8134febf4f4c165bd47665"; // DB password

        $db = pg_connect("host=" . $host . " port=" . $port . " dbname=" . $db_name . " user=" . $db_username . "  password=" . $db_password . " sslmode=Prefer" . " Trust Server Certificate=true") or die("Connection error");
		//Connect to DB
		require dirname(__FILE__) . '/database.php';

        $query = "SELECT count(*) c from public.users WHERE email = '" . $email . "'";
        $result = pg_exec($db, $query);
        if($result)
        {
            echo pg_result($result, 0, 'c');
        }
		else
		{
			$errors[] = "Something went wrong, no result, please try again.";
		}
		
		if(count($errors) > 0)
		{
			echo $errors[0];
		}
	}
	else
	{
		echo "Missing data.";
	}
?>