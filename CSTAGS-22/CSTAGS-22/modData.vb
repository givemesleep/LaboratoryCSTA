Imports System.Data.Odbc
Module modData
    Public con As Odbc.OdbcConnection

    Public Sub dbConnect()
        Try
            con = New Odbc.OdbcConnection("DSN=cstagsv5")
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try
    End Sub




End Module
