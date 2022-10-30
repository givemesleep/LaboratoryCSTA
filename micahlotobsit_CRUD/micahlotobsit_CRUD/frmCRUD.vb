Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call loadDGV("SELECT * FROM products", dgv)
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Call loadDGV("SELECT * FROM products", dgv, "description", Trim(txtsearch.Text))
    End Sub

    Public Sub cleaner(ByVal pnl As Panel)
        For Each obj In pnl.Controls
            If TypeOf obj Is TextBox Then
                obj.Text = ""
            End If
        Next
    End Sub

    Public Sub toggle(ByVal bool As Boolean)
        pnlDataEntry.Enabled = bool
        pnlHeader.Enabled = Not bool
        dgv.Enabled = Not bool

        btnNew.Enabled = Not bool
        btnSave.Enabled = bool
        btnCancel.Enabled = bool
        btnEdit.Enabled = Not bool
        btnDelete.Enabled = Not bool
        btnExit.Enabled = Not bool
    End Sub

    Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        Call toggle(True)
        Call cleaner(pnlDataEntry)
        txtID.Text = 0
        txtBrand.Focus()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If MsgBox("Are you sure you want cancel this operation?", vbYesNo + vbQuestion, "Cancel") = vbYes Then
            Call cleaner(pnlDataEntry)
            Call toggle(False)
        End If
    End Sub

    Private Sub txtUnitPrice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtUnitPrice.KeyPress
        Select Case Asc(e.KeyChar)
            Case Asc("0") To Asc("9")
            Case 8
            Case 13
            Case Asc(".")
                If InStr(txtUnitPrice.Text, ".", CompareMethod.Text) = 0 Then
                    e.KeyChar = e.KeyChar
                Else
                    e.KeyChar = ""
                End If
        End Select
    End Sub

    Function checker(ByVal pnl As Panel) As Boolean
        For Each obj In pnl.Controls
            If TypeOf obj Is TextBox Then
                If Len(obj.text) = 0 Then
                    MsgBox("Please Enter all the data", vbCritical, "Save")
                    obj.Focus()
                    Return False
                    Exit Function
                End If
            End If
        Next
        Return True
    End Function

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim cmd As Odbc.OdbcCommand
        Dim sql As String

        If checker(pnlDataEntry) Then
            Try
                Call connectDB()
                If Val(txtID.Text) = 0 Then
                    sql = "INSERT INTO products(brand, description, unitprice)VALUES(?,?,?)"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", UCase(Trim(txtBrand.Text)))
                    cmd.Parameters.AddWithValue("@", StrConv(Trim(txtDescription.Text), vbProperCase))
                    cmd.Parameters.AddWithValue("@", Val(txtUnitPrice.Text))
                Else
                    sql = "UPDATE products SET brand=?, description=?, unitprice=? WHERE productID=?"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", UCase(Trim(txtBrand.Text)))
                    cmd.Parameters.AddWithValue("@", StrConv(Trim(txtDescription.Text), vbProperCase))
                    cmd.Parameters.AddWithValue("@", Val(txtUnitPrice.Text))
                    cmd.Parameters.AddWithValue("@", Val(txtID.Text))
                End If
                cmd.ExecuteNonQuery()
                Call cleaner(pnlDataEntry)
                Call toggle(False)
                Call loadDGV("SELECT * FROM products", dgv)
            Catch ex As Exception
                MsgBox(ex.Message.ToString)
            Finally
                con.Close()
                GC.Collect
            End Try
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If Val(dgv.Tag) = 0 Then
            MsgBox("Please select a record to delete!", vbExclamation, "DELETE")
        Else
            If MsgBox("Are you sure you want to DELETE the selected record?", vbYesNo + vbQuestion, "DELETE") = vbNo Then
                dgv.Tag = "0"
            Else
                Dim cmd As Odbc.OdbcCommand
                Dim sql As String = "DELETE FROM products WHERE productID=?"
                Try
                    Call connectDB()
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", Val(dgv.Tag))
                    cmd.ExecuteNonQuery()
                    Call loadDGV("SELECT * FROM products", dgv)
                    MsgBox("Record was successfully DELETED!", vbInformation, "DELETE")
                    dgv.Tag = "0"
                Catch ex As Exception
                    MsgBox(ex.Message.ToString, vbCritical, "ERROR")
                Finally
                    con.Close()
                    GC.Collect()
                End Try
            End If
        End If
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Dispose()
    End Sub

    Private Sub loadRec(ByVal id As Integer)
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable
        Dim sql As String = "SELECT * FROM products WHERE productID=?"
        Try
            Call connectDB()
            cmd = New Odbc.OdbcCommand(sql, con)
            cmd.Parameters.AddWithValue("@", id)
            da.SelectCommand = cmd
            da.Fill(dt)
            txtID.Text = dt.Rows(0)(0).ToString
            txtBrand.Text = dt.Rows(0)(1).ToString
            txtDescription.Text = dt.Rows(0)(2).ToString
            txtUnitPrice.Text = dt.Rows(0)(3).ToString
        Catch ex As Exception
            MsgBox(ex.Message.ToString, vbCritical, "Error")
        Finally
            da.Dispose()
            dt.Dispose()
            con.Close()
            GC.Collect
        End Try
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If Val(dgv.Tag) = 0 Then
            MsgBox("Please select a record to edit", vbExclamation, "EDIT")
        Else
            If MsgBox("Are you sure you want to EDIT the selected record?", vbYesNo + vbQuestion, "EDIT") = vbNo Then
                dgv.Tag = "0"
            Else
                Call loadRec(Val(dgv.Tag))
                Call toggle(True)
            End If
        End If
    End Sub

    Private Sub dgv_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv.CellClick
        Try
            dgv.Tag = dgv.Item(0, e.RowIndex).Value
        Catch ex As Exception

        End Try
    End Sub
End Class
