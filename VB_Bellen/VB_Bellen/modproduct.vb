Imports System.Data.Odbc
Module modproduct

    Public con As Odbc.OdbcConnection

    Public Sub connectDB()
        Try
            con = New Odbc.OdbcConnection("DSN=DSNlabtwo")
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            GC.Collect()
        End Try
    End Sub

    Public Sub loadDGV(ByVal sql As String, ByVal dgv As DataGridView, Optional ByVal field As String = "", Optional ByVal value As String = "")
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable

        Try
            Call connectDB()
            If field.Length = 0 Then
                cmd = New Odbc.OdbcCommand(sql, con)
            Else
                sql = sql & " WHERE " & field & " LIKE ? "
                cmd = New Odbc.OdbcCommand(sql, con)
                cmd.Parameters.AddWithValue("@", "%" & value & "%")
            End If

            da.SelectCommand = cmd
            da.Fill(dt)
            dgv.DataSource = dt
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            con.Close()
            GC.Collect()
        End Try
    End Sub

End Module
