Public Class frmproduct

    Private Sub frmproduct_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call loadDGV("SELECT * FROM product", dgv)
    End Sub

    Private Sub btnsearch_Click(sender As Object, e As EventArgs) Handles btnsearch.Click
        Call loadDGV("SELECT * FROM product", dgv, "description", Trim(txtsearch.Text))
    End Sub

    Private Sub toggle(ByVal bool As Boolean)
        pnldataentry.Enabled = bool
        btnnew.Enabled = Not bool
        btnsave.Enabled = bool
        btncancel.Enabled = bool
        btnedit.Enabled = Not bool
        btndelete.Enabled = Not bool
        btnexit.Enabled = Not bool
        dgv.Enabled = bool
        pnlheader.Enabled = Not bool        
    End Sub

    Private Sub cleaner(ByVal pnl As Panel)
        For Each obj As Object In pnl.Controls
            If TypeOf obj Is TextBox Then
                obj.clear()
            End If
        Next
    End Sub

    Private Sub btnnew_Click(sender As Object, e As EventArgs) Handles btnnew.Click
        Call cleaner(pnldataentry)
        Call toggle(True)
        txtproductID.Text = 0
        txtbrand.Focus()
    End Sub

    Private Sub btncancel_Click(sender As Object, e As EventArgs) Handles btncancel.Click
        If MsgBox("Cancel?", vbYesNo + vbQuestion) = vbYes Then
            Call toggle(False)
            Call cleaner(pnldataentry)
        End If
    End Sub


    Private Sub btnsave_Click(sender As Object, e As EventArgs) Handles btnsave.Click
        Dim cmd As Odbc.OdbcCommand
        Dim sql As String
        If checker(pnldataentry) = True Then
            Try
                Call connectDB()
                sql = "INSERT INTO product(brand, description, unit_price)VALUES(?,?,?)"
                cmd = New Odbc.OdbcCommand(sql, con)
                cmd.Parameters.AddWithValue("@", Trim(txtbrand.Text))
                cmd.Parameters.AddWithValue("@", Trim(txtdescription.Text))
                cmd.Parameters.AddWithValue("@", Trim(txtunitprice.Text))
                cmd.ExecuteNonQuery()
                Call cleaner(pnldataentry)
                Call toggle(False)
                Call loadDGV("SELECT * FROM product", dgv)

            Catch ex As Exception
                MsgBox(ex.Message.ToString)
            Finally
                con.Close()
                GC.Collect()
            End Try
        End If
    End Sub

    Function checker(ByVal pnl As Panel) As Boolean
        For Each obj As Object In pnl.Controls
            If TypeOf obj Is TextBox Then
                If Len(obj.text) = 0 Then
                    checker = False
                    MsgBox("Insufficient Data! Please enter all information", vbCritical)
                    obj.focus()
                    Exit Function
                End If
            End If
        Next
        checker = True
    End Function

    Private Sub txtunitprice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtunitprice.KeyPress
        Select Case Asc(e.KeyChar)
            Case Asc("0") To Asc("9")
                e.KeyChar = e.KeyChar
            Case 8
                e.KeyChar = e.KeyChar
            Case Asc(".")
                If InStr(txtunitprice.Text, ".") Then
                    e.KeyChar = ""
                Else
                    e.KeyChar = e.KeyChar
                End If
            Case Else
                e.KeyChar = ""
        End Select
    End Sub

End Class
