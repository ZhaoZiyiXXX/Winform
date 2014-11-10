<!DOCTYPE html>
<html lang="zh-cn">
    <head runat="server">
        <meta charset="utf-8"/>
        <meta http-equiv="X-UA-Compatible" content="IE=edge"/>
        <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no"/>
        <title>test</title>

        <!-- Bootstrap -->
	    <link href="http://cdn.bootcss.com/bootstrap/3.2.0/css/bootstrap.min.css" rel="stylesheet"/>


        <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
        <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
        <!--[if lt IE 9]>
        <script src="http://cdn.bootcss.com/html5shiv/3.7.2/html5shiv.min.js"></script>
        <script src="http://cdn.bootcss.com/respond.js/1.4.2/respond.min.js"></script>
        <![endif]-->

        <!-- jQuery (necessary for Bootstrap's JavaScript plugins) -->
        <script src="http://cdn.bootcss.com/jquery/1.11.1/jquery.min.js"></script>
        <!-- Include all compiled plugins (below), or include individual files as needed -->
        <script src="http://cdn.bootcss.com/bootstrap/3.2.0/js/bootstrap.min.js"></script>

    </head>
	<body>
	    <div class="container-fluid" style="text-align:center">
        <h1>喵校园用户意见反馈板</h1>
            <div class="row">
            <div class="col-lg-10 col-lg-offset-1 col-md-10 col-md-offset-1">
                <table id="result" class="table table-striped">
                <?php  
	                $url='http://1.mallschoolwx.sinaapp.com/outjson/getyijian.php';
	                $html = file_get_contents($url);
	                $data = json_decode($html);
	                echo $data;
	                echo $html;
                ?>
                    <tr>
                        <td>测试</td>
                    </tr>
                </table>
                </div>
            </div>
        </div>
	</body>
</html>