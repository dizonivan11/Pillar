# Pillar
## A CSharp Web Framework

An example usage of the framework.
```csharp
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title>Pillar Site</title>
    <meta name="viewport" content="width=device-width, initial-scale=1">
</head>
<body>
    <a href="info/contact">Contact Us</a><br>
	<csharp-func>
		void SayHello(int times) {
			for (int i = 0; i < times; i++) Echo("Hello World " + (i + 1).ToString() + "<br>");
		}
		void DisplayCurrentDateTime(string format="MM dd, yyyy") {
			Echo(DateTime.Now.ToString(format) + "<br>");
		}
	</csharp-func>
    <csharp>
		if (POST.ContainsKey("fullname")) Echo("Hello " + POST["fullname"] + "!<br><br>");
		else if (GET.ContainsKey("fullname")) Echo("Hello " + GET["fullname"] + "!<br><br>");
		else Echo("Hello our Guest!<br><br>");
        SayHello(10);
		VAR.Add("mySurname", "Dizon");
    </csharp>
	<br><hr>
	<csharp>
		Echo("Variable \"mySurname\" = " + VAR["mySurname"].ToString() + "<br><br>");
		DisplayCurrentDateTime();
		Student me = new Student("14-41280", "Justin Ivan", "Flores", "Dizon");
		Echo("<h3>" + me.FullName() + "</h3>");
		Echo("<h4>" + me.ToString() + "</h4>");
	</csharp>
	<csharp-class>
		class Student {
			public string SrCode;
			public string FirstName;
			public string MiddleName;
			public string LastName;
			
			public Student(string sr, string first, string middle, string last) {
				SrCode = sr;
				FirstName = first;
				MiddleName = middle;
				LastName = last;
			}
			
			public string FullName() {
				return FirstName + " " + MiddleName + " " + LastName;
			}
			
			public override string ToString() {
				return string.Format("This Student has Sr Code: {0}{4}First Name: {1}{4}Middle Name: {2}{4}Last Name: {3}", SrCode, FirstName, MiddleName, LastName, "<br>");
			}
		}
	</csharp-class>
</body>
</html>
```

Post and Get Forms
```html
<!DOCTYPE html>
<html>
	<head>
		<meta charset="utf-8" />
		<meta http-equiv="X-UA-Compatible" content="IE=edge">
		<title>Contact Info</title>
		<meta name="viewport" content="width=device-width, initial-scale=1">
	</head>
	<body>
		<a href="../index">Go Back</a>
		<h1>Contact Us</h1>
		<img src="aesir.png">
		<hr>
		<h2>POST FORM</h2>
		<form action="../index?action=submitted" method="POST">
			Full Name: <input type="text" name="fullname"><br>
			Mobile: <input type="text" name="mobile"><br>
			Feedback: <input type="text" name="feedback"><br>
			<input type="submit" name="btnSubmit">
		</form>
		<hr>
		<h2>GET FORM</h2>
		<form action="../index.phtml" method="GET">
			Full Name: <input type="text" name="fullname"><br>
			Mobile: <input type="text" name="mobile"><br>
			Feedback: <input type="text" name="feedback"><br>
			<input type="submit" name="btnSubmit">
		</form>
	</body>
</html>
```
As the user submits the form, the post/get data will be transferred to a dictionary which can be used by the action page.