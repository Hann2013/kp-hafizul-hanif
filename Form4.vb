Imports System.Data.SqlClient
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Drawing.Printing

Public Class Form4
    Dim WithEvents PD As New PrintDocument
    Dim PPD As New PrintPreviewDialog
    Sub kondisiAwal()
        Using Conn As New SqlConnection(stringConnection)
            Conn.Open()
            Using sda As New SqlDataAdapter("select id_laporan,nama_menu,harga_satuan,qty,tgl_transaksi,total_bayar from Tbl_laporan", Conn)
                Using ds As New DataSet
                    sda.Fill(ds, "Tbl_laporan")
                    DataGridView1.DataSource = (ds.Tables("Tbl_laporan"))
                    Chart1.DataSource = ds.Tables("Tbl_laporan")
                End Using
            End Using
            Conn.Close()
        End Using
    End Sub
    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        kondisiAwal()

    End Sub

    Private Sub TransaksiToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TransaksiToolStripMenuItem.Click
        Me.Visible = False
        Form2.Visible = True
    End Sub

    Private Sub KelolaMenuToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KelolaMenuToolStripMenuItem.Click
        Me.Visible = False
        Form3.Visible = True
    End Sub

    Private Sub LogoutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogoutToolStripMenuItem.Click
        Me.Visible = False
        Form1.Visible = True
    End Sub

    Private Sub btnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select id_laporan,nama_menu,harga_satuan,qty,tgl_transaksi,total_bayar from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end order by Tgl_Transaksi desc", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Using sda As New SqlDataAdapter(cmd)
                    Dim ds As New DataSet
                    sda.Fill(ds, "Tbl_laporan")
                    DataGridView1.DataSource = ds.Tables("Tbl_laporan")
                End Using
            End Using
            conn.Close()
        End Using
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As EventArgs) Handles btnGenerate.Click
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select tgl_Transaksi,total_bayar from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Dim sdr As SqlDataReader = cmd.ExecuteReader
                Me.Chart1.Series("Omset").Points.Clear()
                If sdr.HasRows Then
                    While sdr.Read()
                        Me.Chart1.Series("Omset").XValueType = DataVisualization.Charting.ChartValueType.Date
                        Me.Chart1.Series("Omset").Points.AddXY(sdr("Tgl_Transaksi"), sdr("total_bayar").ToString)
                    End While
                End If
                sdr.Close()
            End Using
            conn.Close()
        End Using
    End Sub


    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Dim printDialog As New PrintDialog()
        If printDialog.ShowDialog() = DialogResult.OK Then
            PD.Print()
        End If
    End Sub

    Private Sub PD_PrintPage(sender As Object, e As PrintPageEventArgs) Handles PD.PrintPage
        Dim FONTJUDUL As New Font("Times New Roman", 18, FontStyle.Bold)
        Dim FONTDATA As New Font("Times New Roman", 11, FontStyle.Regular)
        Dim kanan As New StringFormat
        Dim tengah As New StringFormat
        Dim kiri As New StringFormat
        kanan.Alignment = StringAlignment.Far
        tengah.Alignment = StringAlignment.Center
        kiri.Alignment = StringAlignment.Near
        e.Graphics.DrawString("LAPORAN PENJUALAN", FONTJUDUL, Brushes.Blue, 450, 25, tengah)

        e.Graphics.DrawLine(Pens.Black, 30, 70, 805, 70)

        e.Graphics.DrawString("NOMOR", FONTDATA, Brushes.Black, 100, 75, kanan)

        e.Graphics.DrawString("NAMA BARANG", FONTDATA, Brushes.Black, 180, 75, kiri)

        e.Graphics.DrawString("HARGA SATUAN", FONTDATA, Brushes.Black, 480, 75, kanan)

        e.Graphics.DrawString("JUMLAH BELI", FONTDATA, Brushes.Black, 650, 75, kanan)

        e.Graphics.DrawString("TOTAL HARGA", FONTDATA, Brushes.Black, 800, 75, kanan)

        e.Graphics.DrawLine(Pens.Black, 30, 100, 805, 100)
        Dim baris As Integer = 105
        Dim NOMOR As Integer = 1
        Dim jumlahBeli As Integer
        Using conn As New SqlConnection(stringConnection)
            conn.Open()
            Using cmd As New SqlCommand("select * from Tbl_laporan where CONVERT(date,tgl_transaksi) between @from and @end", conn)
                cmd.Parameters.AddWithValue("@from", Format(DateTimePicker1.Value, "yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@end", Format(DateTimePicker2.Value, "yyyy-MM-dd"))
                Dim dr As SqlDataReader = cmd.ExecuteReader
                dr.Read()
                While dr.Read
                    e.Graphics.DrawString(NOMOR, FONTDATA, Brushes.Black, 100, baris, kanan)

                    e.Graphics.DrawString(dr("nama_menu"), FONTDATA, Brushes.Black, 180, baris, kiri)

                    e.Graphics.DrawString(FormatCurrency(dr("harga_satuan")), FONTDATA, Brushes.Black, 480, baris, kanan)

                    e.Graphics.DrawString(Format(dr("qty"), "#,#"), FONTDATA, Brushes.Black, 650, baris, kanan)

                    e.Graphics.DrawString(FormatCurrency(dr("total_bayar")), FONTDATA, Brushes.Black, 800, baris, kanan)


                    NOMOR = NOMOR + 1

                    baris = baris + 20


                    jumlahBeli += dr("total_bayar")

                End While
                e.Graphics.DrawLine(Pens.Black, 30, baris + 5, 805, baris + 5)

                e.Graphics.DrawString("TOTAL : ", FONTDATA, Brushes.Black, 700, baris + 9, kanan)

                e.Graphics.DrawString(FormatCurrency(jumlahBeli), FONTDATA, Brushes.Black, 793, baris + 9, kanan)


                e.Graphics.DrawLine(Pens.Black, 30, baris + 35, 805, baris + 35)

                dr.Close()
            End Using
        End Using
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub
End Class