Imports System.Configuration
Imports MySql.Data.MySqlClient

Public Class FrmDatabaseSetup
    Dim _conn As MySqlConnection
    Dim _cmd As MySqlCommand
    Dim _dr As MySqlDataReader
    Dim _computerName As String
    Dim _exists As Integer

    Private Sub Showdatabases()
        Try
            Dim strConn As String

            strConn = "Server ='" + txthost.Text + "'; port='" + txtport.Text + "'; userid = '" + txtuser.Text +
                      "'; password = '" + txtpassword.Text +
                      "';"
            strConn &= "Database = mysql; pooling=false;"

            _conn = New MySqlConnection(strConn)

            _cmd = New MySqlCommand("Show Databases", _conn)

            _conn.Open()

            _dr = _cmd.ExecuteReader
            cbodatabase.Properties.Items.Clear()
            If _dr.HasRows Then
                cbodatabase.Properties.Items.Add("Journal263")
                Do While _dr.Read
                    cbodatabase.Properties.Items.Add(_dr.Item("Database").ToString)
                Loop
             End If

            _dr.Close()

        Catch ex As MySqlException
            'MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub txthost_TextChanged(sender As Object, e As EventArgs) Handles txthost.TextChanged
        if txtuser.Text = "" or txthost.Text = "" Then
        Else
            Showdatabases()
        End If
    End Sub

    Private Sub txtuser_TextChanged(sender As Object, e As EventArgs) Handles txtuser.TextChanged
        if txtuser.Text = "" or txthost.Text = "" Then
        Else
            Showdatabases()
        End If
    End Sub

    Private Sub txtpassword_TextChanged(sender As Object, e As EventArgs) Handles txtpassword.TextChanged
        if txtuser.Text = "" or txthost.Text = "" or txtpassword.Text = "" Then
        Else
            Showdatabases()
        End If
    End Sub

    Private Sub SimpleButton1_Click(sender As Object, e As EventArgs) Handles SimpleButton1.Click
        if txtuser.Text = "" or txthost.Text = "" or txtport.Text = "" or cbodatabase.Text = "" Then
            MsgBox("Please ensure no field is empty before saving!!", vbInformation, "Caution")
            Exit Sub
        End If

        Try
            Dim strConn As String
            strConn = "Server ='" + txthost.Text + "';port='" + txtport.Text + "'; userid = '" + txtuser.Text +
                      "'; password = '" + txtpassword.Text +
                      "'; Database = '" + cbodatabase.Text + "'"
            'Create database first
            Dim strConn22 As String
             strConn22 = "Server ='" + txthost.Text + "';port='" + txtport.Text + "'; userid = '" + txtuser.Text +
                      "'; password = '" + txtpassword.Text +
                      "'"
           _conn = New MySqlConnection(strConn22)
            _conn.close
            _conn = New MySqlConnection(strConn22)
            _cmd = New MySqlCommand("Create Database If Not exists Journal263", _conn)
            _conn.Open()
            _cmd.ExecuteScalar()
            _cmd.Dispose()
            _conn.Close()

            _conn.close
            _conn = New MySqlConnection(strConn)
            _cmd =
                New MySqlCommand(
                    " CREATE TABLE if not exists `database_connection` (
  `computer_name` varchar(255) NOT NULL,
  `host` varchar(255) DEFAULT NULL,
  `user` varchar(255) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
   `date_set` timestamp NULL DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP,
  `connection_status` varchar(255) DEFAULT '',
  PRIMARY KEY (`computer_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;",
                    _conn)
            _conn.Open()
            _cmd.ExecuteScalar()
            _cmd.Dispose()
            _conn.Close()


            _conn.close
            _conn = New MySqlConnection(strConn)
            _cmd =
                New MySqlCommand(
                    "CREATE TRIGGER `savedate` BEFORE INSERT ON `database_connection` FOR EACH ROW SET NEW.date_set = NOW()
; CREATE TRIGGER `savedateupdate` BEFORE UPDATE ON `database_connection` FOR EACH ROW SET NEW.date_set = NOW()
",
                    _conn)
            _conn.Open()
            _cmd.ExecuteScalar()
            _cmd.Dispose()
            _conn.Close()

          

            _conn = New MySqlConnection(strConn)

            _conn.Close()
            _conn.Open()
            _cmd =
                New MySqlCommand("select count(*) from database_connection where computer_name='" & _computerName & "' ",
                                 _conn)
            Dim oJobName as object = _cmd.ExecuteScalar()

            If oJobName Is Nothing Then
                Insertdata
                clear()
                Exit Sub
            Else
                _exists = CInt(oJobName.ToString)
            End If
            _cmd.Dispose()
            _conn.Close()
            if _exists = 0
                Insertdata
            Else
                Updatedata
            End If

            clear()
            Dispose()
        Catch ex As Exception
            _cmd.Dispose()
            _conn.Close()
            MsgBox(ex.Message)
        End Try
    End Sub

    Private sub Clear
        txtuser.Text = ""
        txthost.Text = ""
        txtpassword.Text = ""
        cbodatabase.Properties.Items.Clear()
    End sub

    Public Function GetComputerName() As String
        _computerName = Net.Dns.GetHostName
        Return _computerName
    End Function

    Private Sub FrmDatabaseSetup_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        GetComputerName()
    End Sub

    Private sub Insertdata

        dim newvalue as String = "server=" + txthost.Text + ";database=" + cbodatabase.Text + ";uid=" + txtuser.Text +
                                 ";password=" + txtpassword.Text + ";port=" + txtport.Text +";Allow User Variables=True"
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim connectionStringsSection = DirectCast(config.GetSection("connectionStrings"), ConnectionStringsSection)
        connectionStringsSection.ConnectionStrings("myConnection").ConnectionString = newvalue
        config.Save()
        ConfigurationManager.RefreshSection("connectionStrings")

        dim newvalueR as String = "XpoProvider=MySql; " + "server=" + txthost.Text + ";port=" + txtport.Text + ";uid=" +
                                  txtuser.Text + ";password=" + txtpassword.Text + " ;database=" + cbodatabase.Text +
                                  ";Allow User Variables=True;persist security info=true;CharSet=utf8;"
        connectionStringsSection.ConnectionStrings("localhost_journal263_Connection").ConnectionString = newvalueR
        config.Save()
        ConfigurationManager.RefreshSection("connectionStrings")

        _conn.Close()
        _conn.Open()
        _cmd =
            New MySqlCommand(
                "insert into database_connection(computer_name,host,user,password,connection_status) values ('" +
                _computerName +
                "','" + txthost.Text + "','" + txtuser.Text + "','" + txtpassword.Text + "','Active') ", _conn)
        _cmd.ExecuteNonQuery()
        MsgBox("Database connection details saved")
        _cmd.Dispose()
        _conn.Close()
    End sub

    Private sub Updatedata
        dim newvalue as String = "server=" + txthost.Text + ";database=" + cbodatabase.Text + ";uid=" + txtuser.Text +
                                 ";password=" + txtpassword.Text + ";port=" + txtport.Text +";Allow User Variables=True"
        Dim config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
        Dim connectionStringsSection = DirectCast(config.GetSection("connectionStrings"), ConnectionStringsSection)
        connectionStringsSection.ConnectionStrings("myConnection").ConnectionString = newvalue
        config.Save()
        ConfigurationManager.RefreshSection("connectionStrings")

        dim newvalueR as String = "XpoProvider=MySql; " + "server=" + txthost.Text + ";port=" + txtport.Text + ";uid=" +
                                  txtuser.Text + ";password=" + txtpassword.Text + " ;database=" + cbodatabase.Text +
                                  ";Allow User Variables=True;persist security info=true;CharSet=utf8;"
        connectionStringsSection.ConnectionStrings("localhost_journal263_Connection").ConnectionString = newvalueR
        config.Save()
        ConfigurationManager.RefreshSection("connectionStrings")

        _conn.Close()
        _conn.Open()
        _cmd =
            New MySqlCommand(
                "update database_connection set host='" + txthost.Text + "',user='" + txtuser.Text +
                "',password= '" + txtpassword.Text + "' where computer_name='" + _computerName + "' ", _conn)
        _cmd.ExecuteNonQuery()
        MsgBox("Database connection details updated")
        _cmd.Dispose()
        _conn.Close()
    End sub

    Private Sub SimpleButton2_Click(sender As Object, e As EventArgs) Handles SimpleButton2.Click
        Dispose()
    End Sub
End Class