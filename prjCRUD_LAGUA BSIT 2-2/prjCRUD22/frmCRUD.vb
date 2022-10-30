Public Class frmCRUD
    Private Sub frmCRUD_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call loadDGV("SELECT * FROM product", dgv)
    End Sub
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Call loadDGV("SELECT * FROM product", dgv, "description", Trim(txtSearch.Text))
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Call loadDGV("SELECT * FROM product", dgv, "description", Trim(txtSearch.Text))
    End Sub

    Private Sub toggle(ByVal bool As Boolean)
        btnNew.Enabled = Not bool
        btnSave.Enabled = bool
        btnCancel.Enabled = bool
        btnEdit.Enabled = Not bool
        btnDelete.Enabled = Not bool
        btnExit.Enabled = Not bool

        pnlDataEntry.Enabled = bool
        dgv.Enabled = Not bool
        pnlHeader.Enabled = Not bool
    End Sub

    Private Sub cleaner(ByVal pnl As Panel)
        For Each obj As Object In pnl.Controls
            If TypeOf obj Is TextBox Then
                obj.text = ""
            End If
        Next
    End Sub

    Function checker(ByVal pnl As Panel) As Boolean
        For Each obj As Object In pnl.Controls
            If TypeOf obj Is TextBox Then
                If Len(obj.text) = 0 Then
                    checker = False
                    MsgBox("Insufficient Data! Please enter all information.", vbCritical, "SAVE")
                    obj.focus()
                    Return False
                    Exit Function
                End If
            End If
        Next
        Return True
    End Function

      Private Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        Call cleaner(pnlDataEntry)
        Call toggle(True)
        txtID.Text = 0
        txtBrand.Focus()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim cmd As Odbc.OdbcCommand
        'Dim strInsert As String = "INSERT INTO product(brand,description,unit_price)VALUES(?,?,?)"
        'Dim strUpdate As String = "UPDATE product SET brand=?,description=?,unit_price=? WHERE productid=?"
        Dim sql As String
        If checker(pnlDataEntry) = True Then
            Try
                Call connectDB()
                If Val(txtID.Text) = 0 Then
                    sql = "INSERT INTO product(brand,description,unit_price)VALUES(?,?,?)"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", Trim(txtBrand.Text))
                    cmd.Parameters.AddWithValue("@", Trim(txtDescription.Text))
                    cmd.Parameters.AddWithValue("@", Val(txtUnitPrice.Text))
                Else
                    sql = "UPDATE product SET brand=?,description=?,unit_price=? WHERE productid=?"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", Trim(txtBrand.Text))
                    cmd.Parameters.AddWithValue("@", Trim(txtDescription.Text))
                    cmd.Parameters.AddWithValue("@", Val(txtUnitPrice.Text))
                    cmd.Parameters.AddWithValue("@", Val(txtID.Text))
                End If
                cmd.ExecuteNonQuery()
                Call cleaner(pnlDataEntry)
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

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        If MsgBox("Are you sure you want to cancel this operation?", vbYesNo + vbQuestion, "CANCEL") = vbYes Then
            Call cleaner(pnlDataEntry)
            Call toggle(False)
        End If
    End Sub

    Private Sub dgv_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgv.CellClick
        Try
            dgv.Tag = dgv.Item(0, e.RowIndex).Value
        Catch ex As Exception

        End Try
    End Sub

    Private Sub loadRecord(id As Integer)
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable
        Dim str As String = "SELECT * FROM product WHERE productid=?"
        Try
            Call connectDB()
            cmd = New Odbc.OdbcCommand(str, con)
            cmd.Parameters.AddWithValue("@", id)
            da.SelectCommand = cmd
            da.Fill(dt)
            txtID.Text = dt.Rows(0)(0).ToString
            txtBrand.Text = dt.Rows(0)(1).ToString
            txtDescription.Text = dt.Rows(0)(2).ToString
            txtUnitPrice.Text = dt.Rows(0)(3).ToString
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            da.Dispose()
            dt.Dispose()
            con.Close()
            GC.Collect()
        End Try
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If Val(dgv.Tag) = 0 Then
            MsgBox("Please Select a Record to Edit!", vbCritical, "CRUD")
        Else
            If MsgBox("Are you sure you want to edit the selected record?", vbYesNo + vbQuestion, "EDIT") = vbNo Then
                dgv.Tag = "0"
            Else
                Call cleaner(pnlDataEntry)
                Call toggle(True)
                Call loadRecord(Val(dgv.Tag))
            End If
        End If
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Dispose()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim cmd As Odbc.OdbcCommand
        Dim str As String = "DELETE FROM product where productid=?"
        If Val(dgv.Tag) = 0 Then
            MsgBox("Select a Record to Delete!", vbCritical, "DELETE")
        Else
            If MsgBox("Are you sure you want to DELETE the selected record?", vbYesNo + vbQuestion, "DELETE") = vbNo Then
                dgv.Tag = "0"
            Else
                Try
                    Call connectDB()
                    cmd = New Odbc.OdbcCommand(str, con)
                    cmd.Parameters.AddWithValue("@", Val(dgv.Tag))
                    cmd.ExecuteNonQuery()
                    Call loadDGV("SELECT * FROM product", dgv)
                    MsgBox("Record was succesfully DELETED!", vbInformation, "DELETE")
                    dgv.Tag = 0
                Catch ex As Exception
                    MsgBox(ex.Message.ToString)
                Finally
                    con.Close()
                    GC.Collect()
                End Try
            End If
        End If
    End Sub

    Private Sub txtUnitPrice_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtUnitPrice.KeyPress
        Select Case Asc(e.KeyChar)
            Case Asc("0") To Asc("9")
            Case 8 ' backspace key
            Case 13 ' enter key
            Case Asc(".")
                If InStr(txtUnitPrice.Text, ".", CompareMethod.Text) = 0 Then
                    e.KeyChar = e.KeyChar
                Else
                    e.KeyChar = ""
                End If
            Case Else
                e.KeyChar = ""
        End Select
    End Sub
End Class
