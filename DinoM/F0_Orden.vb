Imports Logica.AccesoLogica
Imports Janus.Windows.GridEX
Imports DevComponents.DotNetBar
Imports DevComponents.DotNetBar.Controls
'DESARROLLADO POR: DANNY GUTIERREZ
Public Class F0_Orden

#Region "ATRIBUTOS"
    Dim _Dsencabezado As DataSet
    Dim dt As DataTable
    Dim _Nuevo As Boolean
    Private _Pos As Integer
    Public _tab As SuperTabItem
    Public _modulo As SideNavItem
    Public _nameButton As String
    Dim _CodCliente As Integer = 0
#End Region

#Region "METODOS PRIVADOS"
    Private Sub _PIniciarTodo()

        Me.Text = "ORDEN PEDIDO"
        Me.WindowState = FormWindowState.Maximized

        _MaxLengthTextBox()
        _PFiltrar()
        _PCargarBuscador()
        _PInhabilitar()
        _prAsignarPermisos()

        Dim blah As New Bitmap(New Bitmap(My.Resources.user), 20, 20)
        Dim ico As Icon = Icon.FromHandle(blah.GetHicon())
        Me.Icon = ico
        GroupPanel1.Style.BackColor = Color.FromArgb(13, 71, 161)
        GroupPanel1.Style.BackColor2 = Color.FromArgb(13, 71, 161)
        GroupPanel1.Style.TextColor = Color.White
        JGr_Buscador.Focus()

    End Sub
    Public Sub _MaxLengthTextBox()
        tbPedido.MaxLength = 300
    End Sub

    Private Sub _prAsignarPermisos()

        Dim dtRolUsu As DataTable = L_prRolDetalleGeneral(gi_userRol, _nameButton)

        Dim show As Boolean = dtRolUsu.Rows(0).Item("ycshow")
        Dim add As Boolean = dtRolUsu.Rows(0).Item("ycadd")
        Dim modif As Boolean = dtRolUsu.Rows(0).Item("ycmod")
        Dim del As Boolean = dtRolUsu.Rows(0).Item("ycdel")

        If add = False Then
            btnNuevo.Visible = False
        End If
        If modif = False Then
            btnModificar.Visible = False
        End If
        If del = False Then
            btnEliminar.Visible = False
        End If
    End Sub


    Private Sub _PFiltrar()
        dt = L_fnOrdenGeneral()
        _Pos = 0

        If dt.Rows.Count <> 0 Then
            _PMostrarRegistro(0)
            LblPaginacion.Text = Str(1) + "/" + dt.Rows.Count.ToString
            If dt.Rows.Count > 0 Then
                btnPrimero.Visible = True
                btnAnterior.Visible = True
                btnSiguiente.Visible = True
                btnUltimo.Visible = True
            End If
        End If
    End Sub

    Private Sub _PMostrarRegistro(_N As Integer)
        Dim dt As DataTable = CType(JGr_Buscador.DataSource, DataTable)
        If (IsNothing(CType(JGr_Buscador.DataSource, DataTable))) Then
            Return
        End If
        With JGr_Buscador
            tbCodigo.Text = .GetValue("oanumi").ToString
            tbFechaOrden.Value = .GetValue("oafdoc")
            _CodCliente = .GetValue("oacliente")
            tbCliente.Text = .GetValue("ydrazonsocial").ToString
            tbFechaEntrega.Value = .GetValue("oafentr")
            tbPedido.Text = .GetValue("oaped").ToString

            If (IsDBNull(.GetValue("oafact"))) Then
                lbFecha.Text = ""
            Else
                lbFecha.Text = CType(.GetValue("oafact"), Date).ToString("dd/MM/yyyy")
            End If

            lbHora.Text = IIf(IsDBNull(.GetValue("oahact")), "", .GetValue("oahact").ToString)
            lbUsuario.Text = IIf(IsDBNull(.GetValue("oauact")), "", .GetValue("oauact").ToString)
        End With
    End Sub

    Private Sub _PInhabilitar()
        tbCodigo.ReadOnly = True
        tbFechaOrden.IsInputReadOnly = True
        tbCliente.ReadOnly = True
        tbFechaEntrega.IsInputReadOnly = True
        tbPedido.ReadOnly = True


        btnNuevo.Enabled = True
        btnModificar.Enabled = True
        btnEliminar.Enabled = True
        btnGrabar.Enabled = False

        JGr_Buscador.Enabled = True
        btnGrabar.Image = My.Resources.save
        _PLimpiarErrores()
    End Sub
    Private Sub _PHabilitar()
        tbFechaOrden.IsInputReadOnly = False
        'tbCliente.ReadOnly = False
        tbFechaEntrega.IsInputReadOnly = False
        tbPedido.ReadOnly = False

        btnNuevo.Enabled = False
        btnModificar.Enabled = False
        btnEliminar.Enabled = False
        btnGrabar.Enabled = True
    End Sub

    Private Sub _PLimpiar()
        tbCodigo.Text = String.Empty
        tbFechaOrden.Value = Now.Date
        tbCliente.Text = String.Empty
        tbFechaEntrega.Value = Now.Date
        tbPedido.Text = String.Empty


        LblPaginacion.Text = String.Empty
    End Sub

    Public Function P_Validar() As Boolean
        Dim _Error As Boolean = True
        MEP.Clear()
        If tbCliente.Text.Trim = String.Empty Then
            tbCliente.BackColor = Color.Red
            MEP.SetError(tbCliente, "Seleccione un cliente!".ToUpper)
            _Error = False
        Else
            tbCliente.BackColor = Color.White
            MEP.SetError(tbCliente, String.Empty)
        End If

        If tbPedido.Text.Trim = String.Empty Then
            tbPedido.BackColor = Color.Red
            MEP.SetError(tbPedido, "Debe ingresar el Pedido!".ToUpper)
            _Error = False
        Else
            tbPedido.BackColor = Color.White
            MEP.SetError(tbPedido, String.Empty)
        End If

        MHighlighterFocus.UpdateHighlights()
        Return _Error
    End Function
    Private Sub _PLimpiarErrores()
        MEP.Clear()
        tbPedido.BackColor = Color.White
        tbCliente.BackColor = Color.White
    End Sub



    Private Sub _PCargarBuscador()

        dt = L_fnOrdenGeneral()

        JGr_Buscador.BoundMode = BoundMode.Bound
        JGr_Buscador.DataSource = dt
        JGr_Buscador.RetrieveStructure()

        With JGr_Buscador.RootTable.Columns("oanumi")
            .Visible = True
            .Caption = "Código".ToUpper
            .Width = 100
        End With
        With JGr_Buscador.RootTable.Columns("oafdoc")
            .Visible = True
            .Caption = "Fecha Orden".ToUpper
            .Width = 120
            .TextAlignment = TextAlignment.Far
        End With
        With JGr_Buscador.RootTable.Columns("oacliente")
            .Visible = False
        End With
        With JGr_Buscador.RootTable.Columns("ydrazonsocial")
            .Caption = "Cliente".ToUpper
            .Width = 300
        End With
        With JGr_Buscador.RootTable.Columns("oafentr")
            .Caption = "Fecha Entrega".ToUpper
            .Width = 135
            .TextAlignment = TextAlignment.Far
        End With
        With JGr_Buscador.RootTable.Columns("oaped")
            .Caption = "Pedido".ToUpper
            .Width = 450
            '.HeaderAlignment = Janus.Windows.GridEX.TextAlignment.Center
        End With
        With JGr_Buscador.RootTable.Columns("oaest")
            .Visible = False
        End With
        With JGr_Buscador.RootTable.Columns("oafact")
            .Visible = False
        End With
        With JGr_Buscador.RootTable.Columns("oahact")
            .Visible = False
        End With
        With JGr_Buscador.RootTable.Columns("oauact")
            .Visible = False
        End With

        'Habilitar Filtradores
        With JGr_Buscador
            .DefaultFilterRowComparison = FilterConditionOperator.Contains
            .FilterMode = FilterMode.Automatic
            .FilterRowUpdateMode = FilterRowUpdateMode.WhenValueChanges
            .GroupByBoxVisible = False

            'diseño de la grilla
            JGr_Buscador.VisualStyle = VisualStyle.Office2007
        End With
    End Sub
    Public Function _fnAccesible()
        Return tbFechaOrden.IsInputReadOnly = False
    End Function
    Private Sub MostrarMensajeError(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.WARNING,
                               5000,
                               eToastGlowColor.Red,
                               eToastPosition.TopCenter)

    End Sub
    Private Sub MostrarMensajeOk(mensaje As String)
        ToastNotification.Show(Me,
                               mensaje.ToUpper,
                               My.Resources.OK,
                               5000,
                               eToastGlowColor.Green,
                               eToastPosition.TopCenter)
    End Sub
#End Region

    Private Sub F0_Orden_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _PIniciarTodo()
    End Sub


#Region " Nuevo-Button "
    Private Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        _PNuevoRegistro()
        JGr_Buscador.Enabled = False
    End Sub

    Private Sub _PNuevoRegistro()
        _PHabilitar()
        'btnNuevo.Enabled = True
        _PLimpiar()
        tbCliente.Focus()
        _Nuevo = True
    End Sub

#Region " Grabar-Button "
    Private Sub btnGrabar_Click(sender As Object, e As EventArgs) Handles btnGrabar.Click
        _PGrabarRegistro()
    End Sub

    Private Sub _PGrabarRegistro()
        Dim _Error As Boolean = False
        If P_Validar() Then

            If False Then
                btnGrabar.Tag = 1
                btnGrabar.Refresh()
                Exit Sub
            Else
                btnGrabar.Tag = 0
                btnGrabar.Refresh()
            End If

            If _Nuevo Then
                L_GrabarOrdenPedido(tbCodigo.Text, tbFechaOrden.Value, _CodCliente, tbFechaEntrega.Value, tbPedido.Text)

                'actualizar el grid de buscador
                _PCargarBuscador()

                tbCliente.Focus()
                ToastNotification.Show(Me, "Código ".ToUpper + tbCodigo.Text + " Grabado con éxito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)
                _PLimpiar()
            Else
                L_ModificarOrdenPedido(tbCodigo.Text, tbFechaOrden.Value, _CodCliente, tbFechaEntrega.Value, tbPedido.Text)

                _PCargarBuscador()
                ToastNotification.Show(Me, "Cçodigo ".ToUpper + tbCodigo.Text + " Modificado con éxito.".ToUpper, My.Resources.GRABACION_EXITOSA, 5000, eToastGlowColor.Green, eToastPosition.TopCenter)

                _Nuevo = False 'aumentado danny
                _PInhabilitar()
                _PFiltrar()
            End If
        End If
    End Sub
#End Region

#Region " Cancelar-Button "
    Private Sub BBtn_Cancelar_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        _PSalirRegistro()
    End Sub

    Private Sub _PSalirRegistro()
        If btnGrabar.Enabled = True Then
            _PLimpiar()
            _PInhabilitar()
            _PFiltrar()
            _PCargarBuscador()
        Else
            _tab.Close()
            _modulo.Select()
        End If
    End Sub
#End Region

#Region " Modificar-Button "
    Private Sub btnModificar_Click(sender As Object, e As EventArgs) Handles btnModificar.Click
        _PModificarRegistro()
        JGr_Buscador.Enabled = False
    End Sub

    Private Sub _PModificarRegistro()
        _Nuevo = False
        _PHabilitar()
        'btnModificar.Enabled = True 'aumentado para q funcione con el modelo de guido
    End Sub
#End Region

#Region " Eliminar-Button "
    Private Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        _PEliminarRegistro()
    End Sub

    Private Sub _PEliminarRegistro()
        Dim ef = New Efecto

        ef.tipo = 2
        ef.Context = "¿esta seguro de eliminar el registro?".ToUpper
        ef.Header = "mensaje principal".ToUpper
        ef.ShowDialog()
        Dim bandera As Boolean = False
        bandera = ef.band
        If (bandera = True) Then
            Dim t As String = tbCodigo.Text
            Dim mensajeError As String = ""
            Dim res As Boolean = L_fnEliminarOrden(tbCodigo.Text, mensajeError)
            If res Then
                _PInhabilitar()
                _PFiltrar()
                _PCargarBuscador()

                Dim img As Bitmap = New Bitmap(My.Resources.checked, 50, 50)

                ToastNotification.Show(Me, "Código de Orden ".ToUpper + t + " eliminado con éxito.".ToUpper,
                                          img, 2000,
                                          eToastGlowColor.Green,
                                          eToastPosition.TopCenter)

            Else
                Dim img As Bitmap = New Bitmap(My.Resources.cancel, 50, 50)
                ToastNotification.Show(Me, mensajeError, img, 2000, eToastGlowColor.Red, eToastPosition.BottomCenter)
            End If
        Else
                _PInhabilitar()
        End If



    End Sub
#End Region

#End Region

    Private Sub BBtn_Inicio_Click(sender As Object, e As EventArgs) Handles btnPrimero.Click

        _PPrimerRegistro()

    End Sub

    Private Sub _PPrimerRegistro()
        Dim _MPos As Integer
        If JGr_Buscador.RowCount > 0 Then
            _MPos = 0
            ''   _prMostrarRegistro(_MPos)
            JGr_Buscador.Row = _MPos
        End If
        LblPaginacion.Text = Str(1) + "/" + CType(JGr_Buscador.DataSource, DataTable).Rows.Count.ToString
    End Sub

    Private Sub BBtn_Ultimo_Click(sender As Object, e As EventArgs) Handles btnUltimo.Click
        Dim _pos As Integer = JGr_Buscador.Row
        If JGr_Buscador.RowCount > 0 Then
            _pos = JGr_Buscador.RowCount - 1
            ''  _prMostrarRegistro(_pos)
            JGr_Buscador.Row = _pos
            LblPaginacion.Text = Str(JGr_Buscador.RowCount).Trim + "/" + Str(JGr_Buscador.RowCount).Trim
        End If
    End Sub


    Private Sub BBtn_Anterior_Click(sender As Object, e As EventArgs) Handles btnAnterior.Click
        Dim _MPos As Integer = JGr_Buscador.Row
        If _MPos > 0 And JGr_Buscador.RowCount > 0 Then
            _MPos = _MPos - 1
            ''  _prMostrarRegistro(_MPos)
            JGr_Buscador.Row = _MPos
            LblPaginacion.Text = Str(_Pos + 1) + "/" + CType(JGr_Buscador.DataSource, DataTable).Rows.Count.ToString

        End If
    End Sub

    Private Sub BBtn_Siguiente_Click(sender As Object, e As EventArgs) Handles btnSiguiente.Click
        Dim _pos As Integer = JGr_Buscador.Row
        If _pos < JGr_Buscador.RowCount - 1 Then
            _pos = JGr_Buscador.Row + 1
            '' _prMostrarRegistro(_pos)
            JGr_Buscador.Row = _pos
            LblPaginacion.Text = Str(_pos + 1) + "/" + JGr_Buscador.RowCount.ToString
        End If
    End Sub



    Private Sub JGr_Buscador_SelectionChanged(sender As Object, e As EventArgs) Handles JGr_Buscador.SelectionChanged
        If JGr_Buscador.Row >= 0 Then
            _PMostrarRegistro(JGr_Buscador.Row)
            LblPaginacion.Text = Str(JGr_Buscador.Row + 1) + "/" + dt.Rows.Count.ToString
        End If
    End Sub

    Private Sub JGr_Buscador_EditingCell(sender As Object, e As EditingCellEventArgs) Handles JGr_Buscador.EditingCell
        e.Cancel = True
    End Sub

    Private Function SuperTabControl1() As Object
        Throw New NotImplementedException
    End Function

    Private Function SuperTabControlPanel1() As Object
        Throw New NotImplementedException
    End Function

    Private Sub tbCliente_KeyDown(sender As Object, e As KeyEventArgs) Handles tbCliente.KeyDown
        Try
            If (_fnAccesible()) Then
                If e.KeyData = Keys.Control + Keys.Enter Then
                    Dim dt As DataTable
                    dt = L_fnListarClientes()

                    Dim listEstCeldas As New List(Of Modelo.Celda)
                    listEstCeldas.Add(New Modelo.Celda("ydnumi,", True, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("ydcod", False, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("ydrazonsocial", True, "RAZON SOCIAL", 180))
                    listEstCeldas.Add(New Modelo.Celda("yddesc", True, "NOMBRE", 280))
                    listEstCeldas.Add(New Modelo.Celda("yddctnum", True, "N. Documento".ToUpper, 150))
                    listEstCeldas.Add(New Modelo.Celda("yddirec", True, "DIRECCION", 220))
                    listEstCeldas.Add(New Modelo.Celda("ydtelf1", True, "Telefono".ToUpper, 200))
                    listEstCeldas.Add(New Modelo.Celda("ydfnac", True, "F.Nacimiento".ToUpper, 150, "MM/dd,YYYY"))
                    listEstCeldas.Add(New Modelo.Celda("ydnumivend,", False, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("vendedor,", False, "ID", 50))
                    listEstCeldas.Add(New Modelo.Celda("yddias", False, "CRED", 50))
                    Dim ef = New Efecto
                    ef.tipo = 3
                    ef.dt = dt
                    ef.SeleclCol = 2
                    ef.listEstCeldas = listEstCeldas
                    ef.alto = 50
                    ef.ancho = 350
                    ef.Context = "Seleccione Cliente".ToUpper
                    ef.ShowDialog()
                    Dim bandera As Boolean = False
                    bandera = ef.band
                    If (bandera = True) Then
                        Dim Row As Janus.Windows.GridEX.GridEXRow = ef.Row

                        _CodCliente = Row.Cells("ydnumi").Value
                        tbCliente.Text = Row.Cells("ydrazonsocial").Value

                    End If

                End If

            End If
        Catch ex As Exception
            MostrarMensajeError(ex.Message)
        End Try
    End Sub
End Class