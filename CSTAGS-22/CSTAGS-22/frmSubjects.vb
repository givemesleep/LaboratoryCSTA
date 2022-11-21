Public Class frmSubjects

    Private Sub frmSubjects_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Call loadDGV("SELECT LPAD(subID,4,'0') subID,scode,stitle,unitlec,unitlab,unitDisplay FROM subjects", "")
    End Sub


    Private Sub loadDGV(ByVal sql As String, ByVal strSearch As String)
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable
        Try
            Call dbConnect()
            cmd = New Odbc.OdbcCommand(sql, con)
            If Len(strSearch) <> 0 Then
                cmd.Parameters.AddWithValue("@", strSearch)
            End If
            da.SelectCommand = cmd
            da.Fill(dt)
            dgv.DataSource = dt
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            da.Dispose()
            con.Close()
            GC.Collect()
        End Try
    End Sub

    Private Sub btnSearch_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSearch.Click
        Call loadDGV("SELECT * FROM subjects WHERE stitle like ?", "%" & Trim(txtSearch.Text) & "%")
    End Sub
    Private Sub cleaner()
        txtCode.Text = ""
        txtTitle.Text = ""
        txtLec.Text = ""
        txtLab.Text = ""
        txtTotal.Text = ""
    End Sub
    Private Sub enabler()
        btnNew.Enabled = True
        btnSave.Enabled = False
        btnCancel.Enabled = False
        btnEdit.Enabled = True
        btnDelete.Enabled = True
        btnExit.Enabled = True
        pnlDE.Enabled = False
        btnSearch.Enabled = True
        dgv.Enabled = True
    End Sub
    Private Sub disabler()
        btnNew.Enabled = False
        btnSave.Enabled = True
        btnCancel.Enabled = True
        btnEdit.Enabled = False
        btnDelete.Enabled = False
        btnExit.Enabled = False
        pnlDE.Enabled = True
        btnSearch.Enabled = False
        dgv.Enabled = False
    End Sub

    Private Sub btnNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNew.Click
        Call disabler()
        Call cleaner()
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        If MsgBox("Cancel?", vbQuestion + vbYesNo, "CANCEL") = vbYes Then
            Call cleaner()
            Call enabler()
        End If
    End Sub

    Private Sub btnSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSave.Click
        Dim cmd As Odbc.OdbcCommand
        Dim sql As String
        If Len(txtCode.Text) = 0 Then
            MsgBox("Please enter subject code", vbCritical, "SAVE")
            txtCode.Focus()
        ElseIf Len(txtTitle.Text) = 0 Then
            MsgBox("Please enter desctiptive title", vbCritical, "SAVE")
            txtTitle.Focus()
        ElseIf Len(txtLec.Text) = 0 Then
            MsgBox("Please enter lecture unit/s", vbCritical, "SAVE")
            txtLec.Focus()
        ElseIf Len(txtLab.Text) = 0 Then
            MsgBox("Please enter laboratory unit/s", vbCritical, "SAVE")
            txtLab.Focus()
        ElseIf Len(txtTotal.Text) = 0 Then
            MsgBox("Please enter the total number of units", vbCritical, "SAVE")
            txtTotal.Focus()
        Else
            Try
                Call dbConnect()
                sql = "INSERT INTO subjects(scode,stitle,unitLec,unitLab,unitDisplay)VALUES(?,?,?,?,?)"
                cmd = New Odbc.OdbcCommand(sql, con)
                With cmd.Parameters
                    .AddWithValue("@", UCase(Trim(txtCode.Text)))
                    .AddWithValue("@", Trim(txtTitle.Text))
                    .AddWithValue("@", Val(txtLec.Text))
                    .AddWithValue("@", Val(txtLab.Text))
                    .AddWithValue("@", Val(txtTotal.Text))
                End With
                cmd.ExecuteNonQuery()
                Call loadDGV("SELECT * FROM subjects", "")
                Call cleaner()
                Call enabler()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub txtLec_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtLec.KeyPress
        Select Case Asc(e.KeyChar)
            Case 8 'Backspace
            Case 13 'Enter 
            Case Asc("0") To Asc("9")
            Case Else
                e.KeyChar = ""
        End Select
    End Sub

    Private Sub txtLec_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLec.TextChanged
        If Len(txtLec.Text) <> 0 And Len(txtLab.Text) <> 0 Then
            txtTotal.Text = Val(txtLec.Text) + Val(txtLab.Text)
        End If
    End Sub

    Private Sub txtLab_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtLab.KeyPress
        Select Case Asc(e.KeyChar)
            Case 8 'Backspace
            Case 13 'Enter 
            Case Asc("0") To Asc("9")
            Case Else
                e.KeyChar = ""
        End Select
    End Sub

    Private Sub txtLab_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtLab.TextChanged
        If Len(txtLec.Text) <> 0 And Len(txtLab.Text) <> 0 Then
            txtTotal.Text = Val(txtLec.Text) + Val(txtLab.Text)
        End If
    End Sub

    Private Sub txtTotal_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtTotal.KeyPress
        Select Case Asc(e.KeyChar)
            Case 8 'Backspace
            Case 13 'Enter 
            Case Asc("0") To Asc("9")
            Case Else
                e.KeyChar = ""
        End Select
    End Sub

    Private Sub dgv_CellClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgv.CellClick
        Try
            dgv.Tag = Val(dgv.Item(0, e.RowIndex).Value)
            Call loader(Val(dgv.Tag))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub loader(ByVal id As Integer)
        Dim cmd As Odbc.OdbcCommand
        Dim da As New Odbc.OdbcDataAdapter
        Dim dt As New DataTable
        Dim sql As String
        Try
            Call dbConnect()
            sql = "SELECT * FROM subjects WHERE subID=?"
            cmd = New Odbc.OdbcCommand(sql, con)
            cmd.Parameters.AddWithValue("@", id)
            da.SelectCommand = cmd
            da.Fill(dt)
            txtCode.Text = dt.Rows(0)(1).ToString
            txtTitle.Text = dt.Rows(0)(2).ToString
            txtLec.Text = dt.Rows(0)(3).ToString
            txtLab.Text = dt.Rows(0)(4).ToString
            txtTotal.Text = dt.Rows(0)(5).ToString
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        Finally
            da.Dispose()
            con.Close()
            GC.Collect()
        End Try
    End Sub

    Private Sub dgv_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgv.CellContentClick

    End Sub

    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click
        Dim cmd As Odbc.OdbcCommand
        Dim sql As String
        If Val(dgv.Tag) = 0 Then
            MsgBox("Please select a record to delete!", vbCritical, "DELETE")
        Else
            If MsgBox("Are you sure you want to delete the selected record?", vbYesNo + vbQuestion, "DELETE") = vbYes Then
                Try
                    Call dbConnect()
                    sql = "DELETE FROM subjects WHERE subID=?"
                    cmd = New Odbc.OdbcCommand(sql, con)
                    cmd.Parameters.AddWithValue("@", Val(dgv.Tag))
                    cmd.ExecuteNonQuery()
                    Call cleaner()
                    dgv.Tag = ""
                    Call loadDGV("SELECT LPAD(subID,4,'0') subID,scode,stitle,unitlec,unitlab,unitDisplay FROM subjects", "")
                Catch ex As Exception
                    MsgBox(ex.Message.ToString)
                Finally
                    con.Close()
                    GC.Collect()
                End Try
            End If
        End If
    End Sub
End Class
