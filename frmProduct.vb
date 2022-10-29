Public Class frmProduct
    Private Sub frmProduct_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call loadDGV("SELECT * FROM tblproduct", dgvProduct)
    End Sub
    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Call loadDGV("SELECT * FROM tblproduct", dgvProduct, "description", Trim(txtSearch.Text))
    End Sub
    Private Sub txtSearch_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtSearch.TextChanged
        Call loadDGV("SELECT * FROM tblproduct", dgvProduct, "description", Trim(txtSearch.Text))
    End Sub
    Private Sub cleaner(ByVal pnl As Panel)
        For Each obj In pnl.Controls
            If TypeOf obj Is TextBox Then
                obj.text = ""
            End If
        Next
    End Sub
    Private Sub toggle(ByVal bool As Boolean)
        pnlDataEntry.Enabled = bool
        pnlHeader.Enabled = Not bool
        dgvProduct.Enabled = Not bool

        btnNew.Enabled = Not bool
        btnSave.Enabled = bool
        btnCancel.Enabled = bool
        btnEdit.Enabled = Not bool
        btnDelete.Enabled = Not bool
        btnExit.Enabled = Not bool
    End Sub
    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        Call cleaner(pnlDataEntry)
        Call toggle(True)
        txtID.Text = 0
        txtBrand.Focus()
    End Sub
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If MsgBox("Are you sure you want to cancel this operation?", vbYesNo + vbQuestion, "CANCEL") = vbYes Then
            Call cleaner(pnlDataEntry)
            Call toggle(False)

        End If
    End Sub
    Private Sub txtPrice_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtPrice.KeyPress
        Select Case Asc(e.KeyChar)
            Case Asc("0") To Asc("9")
            Case 8 ' back space 
            Case 13 ' enter 
            Case Asc(".")
                If InStr(txtPrice.Text, ".", CompareMethod.Text) = 0 Then
                    e.KeyChar = e.KeyChar
                Else
                    e.KeyChar = ""
                End If
            Case Else
                e.KeyChar = ""
        End Select
    End Sub
    Function checker(ByVal pnl As Panel) As Boolean
        For Each obj In pnl.Controls
            If TypeOf obj Is TextBox Then
                If Len(obj.text) = 0 Then
                    MsgBox("Insufficient Data!", vbCritical, "SAVE")
                    obj.focus()
                    Return False
                    Exit Function
                End If
            End If
        Next
        Return True
    End Function
   
    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim cmd As Odbc.OdbcCommand
        Dim sql As String
        If checker(pnlDataEntry) = True Then
            Try
                Call connectDB()
                If Val(txtID.Text) = 0 Then
                    sql = "INSERT INTO tblproduct(brand,description,unitprice)VALUES(?,?,?)"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", UCase(Trim(txtBrand.Text)))
                    cmd.Parameters.AddWithValue("@", StrConv(Trim(txtDesc.Text), vbProperCase))
                    cmd.Parameters.AddWithValue("@", Val(txtPrice.Text))
                Else
                    sql = "UPDATE tblproduct SET brand=?,description=?,unitprice=? WHERE prodID=?"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", UCase(Trim(txtBrand.Text)))
                    cmd.Parameters.AddWithValue("@", StrConv(Trim(txtDesc.Text), vbProperCase))
                    cmd.Parameters.AddWithValue("@", Val(txtPrice.Text))
                    cmd.Parameters.AddWithValue("@", Val(txtID.Text))
                End If
                cmd.ExecuteNonQuery()
                Call cleaner(pnlDataEntry)
                Call toggle(False)
                Call loadDGV("SELECT * FROM tblproduct", dgvProduct)
            Catch ex As Exception
                MsgBox(ex.Message.ToString, vbCritical, "ERROR")
            Finally
                con.Close()
                GC.Collect()
            End Try
        End If
    End Sub

    

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        If Val(dgvProduct.Tag) = 0 Then
            MsgBox("Please select a record to delete!", vbExclamation, "DELETE")
        Else
            If MsgBox("Are you sure you want to DELETE the selected record?", vbYesNo + vbQuestion, "DELETE") = vbNo Then
                dgvProduct.Tag = "0"
            Else
                Dim cmd As Odbc.OdbcCommand
                Dim sql As String = "DELETE FROM tblproduct WHERE prodID=?"
                Try
                    Call connectDB()
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", Val(dgvProduct.Tag))
                    cmd.ExecuteNonQuery()
                    Call loadDGV("SELECT * FROM tblproduct", dgvProduct)
                    MsgBox("Record was succesfully DELETED!", vbInformation, "DELETE")
                    dgvProduct.Tag = "0"
                Catch ex As Exception
                    MsgBox(ex.Message.ToString, vbCritical, "ERROR")
                Finally
                    con.Close()
                    GC.Collect()
                End Try
            End If
        End If
    End Sub

    Private Sub btnExit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnExit.Click
        Me.Dispose()
    End Sub
    Private Sub loadRec(ByVal id As Integer)
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable
        Dim sql As String = "SELECT * FROM tblproduct WHERE prodID=?"
        Try
            Call connectDB()
            cmd = New Odbc.OdbcCommand(sql, con)
            cmd.Parameters.AddWithValue("@", id)
            da.SelectCommand = cmd
            da.Fill(dt)
            txtID.Text = dt.Rows(0)(0).ToString
            txtBrand.Text = dt.Rows(0)(1).ToString
            txtDesc.Text = dt.Rows(0)(2).ToString
            txtPrice.Text = dt.Rows(0)(3).ToString
        Catch ex As Exception
            MsgBox(ex.Message.ToString, vbCritical, "ERROR")
        Finally
            da.Dispose()
            dt.Dispose()
            con.Close()
            GC.Collect()
        End Try
    End Sub
    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        If Val(dgvProduct.Tag) = 0 Then
            MsgBox("Please select a record to edit!", vbExclamation, "EDIT")
        Else
            If MsgBox("Are you sure you want to EDIT the selected record?", vbYesNo + vbQuestion, "EDIT") = vbNo Then
                dgvProduct.Tag = "0"
            Else
                Call loadRec(Val(dgvProduct.Tag))
                Call toggle(True)
            End If
        End If
    End Sub

    Private Sub dgvProduct_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvProduct.CellClick
        Try
            dgvProduct.Tag = dgvProduct.Item(0, e.RowIndex).Value
        Catch ex As Exception

        End Try
    End Sub

End Class
