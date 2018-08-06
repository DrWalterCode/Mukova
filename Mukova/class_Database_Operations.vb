
Imports System.Configuration
Imports MySql.Data.MySqlClient

Public Class class_Database_Operations
    ReadOnly _
        _con As MySqlConnection =
            New MySqlConnection(ConfigurationManager.ConnectionStrings("myConnection").ConnectionString)

    Dim _cmd As MySqlCommand

    Public Sub Save(ByVal sql As String, ByVal parameters() As String, ByVal values() As String)
        Try
            _con.Close()
            _con.Open()
            _cmd = New MySqlCommand(sql, _con)

            For i = 0 To parameters.Count - 1
                _cmd.Parameters.AddWithValue("@" & parameters(i).ToString, values(i))
            Next
            _cmd.CommandText = sql
            _cmd.ExecuteNonQuery()
            _con.Close()
            MsgBox("Saved Successfully", vbInformation, "Success")
        Catch ex As Exception
            MsgBox(ex.Message)
            _cmd.Dispose()
            _con.Close()
        End Try
    End Sub

    Public Sub Update(ByVal sql As String, ByVal parameters() As String, ByVal values() As String)
        Try
            _con.Close()
            _con.Open()
            _cmd = New MySqlCommand(sql, _con)

            For i = 0 To parameters.Count - 1
                _cmd.Parameters.AddWithValue("@" & parameters(i).ToString, values(i))
            Next
            _cmd.CommandText = sql
            _cmd.ExecuteNonQuery()
            _con.Close()
            MsgBox("Updated Successfully", vbInformation, "Success")
        Catch ex As Exception
            MsgBox(ex.Message)
            _cmd.Dispose()
            _con.Close()
        End Try
    End Sub

    Public Sub Delete(ByVal sql As String, ByVal parameters() As String, ByVal values() As String)
        Try
            _con.Close()
            _con.Open()
            _cmd = New MySqlCommand(sql, _con)

            For i = 0 To parameters.Count - 1
                _cmd.Parameters.AddWithValue("@" & parameters(i).ToString, values(i))
            Next
            _cmd.CommandText = sql
            _cmd.ExecuteNonQuery()
            _con.Close()
            MsgBox("Deleted Successfully", vbInformation, "Success")
        Catch ex As Exception
            MsgBox(ex.Message)
            _cmd.Dispose()
            _con.Close()
        End Try
    End Sub

    Public Function GetData(ByVal cmd As MySqlCommand) As DataTable
        Dim dt As New DataTable
        Dim sda As New MySqlDataAdapter
        cmd.CommandType = CommandType.Text
        cmd.Connection = _con
        Try
            _con.Open()
            sda.SelectCommand = cmd
            sda.Fill(dt)
            Return dt
        Catch ex As Exception
            MsgBox(ex.Message)
            Return Nothing
        Finally
            _con.Close()
            sda.Dispose()
            _con.Dispose()
        End Try
    End Function
End Class
