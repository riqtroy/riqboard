using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using RIQBoards;
using System.IO;
using System.Net;
using System.Text;

namespace RIQBoard
{
    public partial class RIQBoardForm : Form
    {
        /* drag and drop form trick on spectate area
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT && move == true)
                message.Result = (IntPtr)HTCAPTION;
        }
        */
        #region declaration
        int winLocX, winLocY, winW, winH, riqW, riqH, riqStep, firstSec, lastSec, currSec, currMin, apm, apmPlus, period, minusW, minusH;
        bool riqMove, apmBool;
        string apmString, version;
        Keys riqMoveKey;
        NotifyIcon riqNotifyIcon;
        ContextMenu riqContextMenu;
        PrivateFontCollection fontSteelFish;
        FontFamily ff;
        Font font;
        Color color;
        Label label_1, label_2, label_3;
        RIQBoardHook riqHook;
        PictureBox riq, riq_layout, riq_update, riq_0, riq_1, riq_2, riq_3, riq_4, riq_5, riq_6, riq_7, riq_8, riq_9, riq_a, riq_alt_left, riq_alt_right, riq_apostrophe, riq_b, riq_backslash,
            riq_backspace, riq_bracket_left, riq_bracket_right, riq_c, riq_caps_lock, riq_comma, riq_control_left, riq_control_right, riq_d, riq_delete, riq_dot,
            riq_down, riq_e, riq_end, riq_enter, riq_equal, riq_escape, riq_f, riq_f1, riq_f2, riq_f3, riq_f4, riq_f5, riq_f6, riq_f7, riq_f8, riq_f9, riq_f10,
            riq_f11, riq_f12, riq_g, riq_h, riq_home, riq_i, riq_insert, riq_j, riq_k, riq_l, riq_left, riq_list, riq_m, riq_minus, riq_n, riq_num_lock,
            riq_num_lock_0, riq_num_lock_1, riq_num_lock_2, riq_num_lock_3, riq_num_lock_4, riq_num_lock_5, riq_num_lock_6, riq_num_lock_7, riq_num_lock_8,
            riq_num_lock_9, riq_num_lock_asterisk, riq_num_lock_dot, riq_num_lock_enter, riq_num_lock_minus, riq_num_lock_plus, riq_num_lock_slash, riq_o, riq_oemtilde,
            riq_p, riq_page_down, riq_page_up, riq_pause, riq_print_screen, riq_q, riq_r, riq_right, riq_s, riq_scroll_lock, riq_semicolon, riq_shift_left, riq_shift_right,
            riq_slash, riq_space, riq_t, riq_tab, riq_u, riq_up, riq_v, riq_w, riq_windows_left, riq_windows_right, riq_x, riq_y, riq_z;
        #endregion
        public RIQBoardForm()
        {
            InitializeComponent();
        }
        private void RIQBoardLoad(object sender, EventArgs e)
        {
            #region assignment
            riqStep = 20; //form moving step size
            riqW = 597; //riq image width, stream + 162
            riqH = 172; //riq image height, stream + 33
            minusW = 12; //minus width
            minusH = 25; //minus height
            riqMoveKey = Keys.F9; //move key
            period = 5; //period of apm
            version = "22032015";

            this.ShowInTaskbar = false; //hide from taskbar
            this.FormBorderStyle = FormBorderStyle.None; //borderless window
            this.BackColor = Color.LimeGreen; //transparency
            this.TransparencyKey = Color.LimeGreen; //transparency
            this.Size = new Size(riqW, riqH); //changing form size to riq size
            winW = Screen.PrimaryScreen.Bounds.Width;
            winH = Screen.PrimaryScreen.Bounds.Height;
            winLocX = winW / 2 - riqW / 2;
            winLocY = winH / 2 - riqH / 2;
            this.Location = new Point(winLocX, winLocY);
            this.Focus();

            riqNotifyIcon = new NotifyIcon();
            riqNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            riqNotifyIcon.BalloonTipText = "RIQBoard";
            riqNotifyIcon.BalloonTipTitle = "RIQBoard";
            riqNotifyIcon.Icon = RIQBoard.Properties.Resources.riqboardlogo;
            riqNotifyIcon.Text = "RIQBoard";
            riqNotifyIcon.Visible = true;
            riqContextMenu = new ContextMenu();
            riqContextMenu.MenuItems.Add("Hide", HideMe);
            riqContextMenu.MenuItems.Add("Show", ShowMe);
            riqContextMenu.MenuItems.Add("Always on top", TopMe);
            riqContextMenu.MenuItems.Add("Help", Help);
            riqContextMenu.MenuItems.Add("Donate", Donate);
            riqContextMenu.MenuItems.Add("Exit", RiqClick);
            riqNotifyIcon.ContextMenu = riqContextMenu;

            riqHook = new RIQBoardHook();
            riqHook.Down += new KeyEventHandler(Down);//handle key down global
            riqHook.Up += new KeyEventHandler(Up);//handle key up global
            this.KeyUp += new KeyEventHandler(FormUp); //handle key up
            this.KeyDown += new KeyEventHandler(FormDown); //handle key down

            byte[] fontArray = RIQBoard.Properties.Resources.steelfish_rg;
            int dataLength = RIQBoard.Properties.Resources.steelfish_rg.Length;
            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);
            Marshal.Copy(fontArray, 0, ptrData, dataLength);
            uint cFonts = 0;
            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);
            fontSteelFish = new PrivateFontCollection();
            fontSteelFish.AddMemoryFont(ptrData, dataLength);
            Marshal.FreeCoTaskMem(ptrData);
            ff = fontSteelFish.Families[0];
            font = new Font(ff, 21f, FontStyle.Regular);
            color = new Color();
            color = ColorTranslator.FromHtml("#55c818");
            #endregion
            #region pictures
            riq_0 = new PictureBox(); riq_0.Image = RIQBoard.Properties.Resources.riq0; riq_0.Size = new Size(riq_0.Image.Width, riq_0.Image.Height); riq_0.Location = new Point(281 - minusW, 62 - minusH); Controls.Add(riq_0); riq_0.Hide();
            riq_1 = new PictureBox(); riq_1.Image = RIQBoard.Properties.Resources.riq1; riq_1.Size = new Size(riq_1.Image.Width, riq_1.Image.Height); riq_1.Location = new Point(44 - minusW, 62 - minusH); Controls.Add(riq_1); riq_1.Hide();
            riq_2 = new PictureBox(); riq_2.Image = RIQBoard.Properties.Resources.riq2; riq_2.Size = new Size(riq_2.Image.Width, riq_2.Image.Height); riq_2.Location = new Point(71 - minusW, 62 - minusH); Controls.Add(riq_2); riq_2.Hide();
            riq_3 = new PictureBox(); riq_3.Image = RIQBoard.Properties.Resources.riq3; riq_3.Size = new Size(riq_3.Image.Width, riq_3.Image.Height); riq_3.Location = new Point(97 - minusW, 62 - minusH); Controls.Add(riq_3); riq_3.Hide();
            riq_4 = new PictureBox(); riq_4.Image = RIQBoard.Properties.Resources.riq4; riq_4.Size = new Size(riq_4.Image.Width, riq_4.Image.Height); riq_4.Location = new Point(124 - minusW, 62 - minusH); Controls.Add(riq_4); riq_4.Hide();
            riq_5 = new PictureBox(); riq_5.Image = RIQBoard.Properties.Resources.riq5; riq_5.Size = new Size(riq_5.Image.Width, riq_5.Image.Height); riq_5.Location = new Point(150 - minusW, 62 - minusH); Controls.Add(riq_5); riq_5.Hide();
            riq_6 = new PictureBox(); riq_6.Image = RIQBoard.Properties.Resources.riq6; riq_6.Size = new Size(riq_6.Image.Width, riq_6.Image.Height); riq_6.Location = new Point(176 - minusW, 62 - minusH); Controls.Add(riq_6); riq_6.Hide();
            riq_7 = new PictureBox(); riq_7.Image = RIQBoard.Properties.Resources.riq7; riq_7.Size = new Size(riq_7.Image.Width, riq_7.Image.Height); riq_7.Location = new Point(202 - minusW, 62 - minusH); Controls.Add(riq_7); riq_7.Hide();
            riq_8 = new PictureBox(); riq_8.Image = RIQBoard.Properties.Resources.riq8; riq_8.Size = new Size(riq_8.Image.Width, riq_8.Image.Height); riq_8.Location = new Point(229 - minusW, 62 - minusH); Controls.Add(riq_8); riq_8.Hide();
            riq_9 = new PictureBox(); riq_9.Image = RIQBoard.Properties.Resources.riq9; riq_9.Size = new Size(riq_9.Image.Width, riq_9.Image.Height); riq_9.Location = new Point(255 - minusW, 62 - minusH); Controls.Add(riq_9); riq_9.Hide();
            riq_a = new PictureBox(); riq_a.Image = RIQBoard.Properties.Resources.riqA; riq_a.Size = new Size(riq_a.Image.Width, riq_a.Image.Height); riq_a.Location = new Point(64 - minusW, 115 - minusH); Controls.Add(riq_a); riq_a.Hide();
            riq_alt_left = new PictureBox(); riq_alt_left.Image = RIQBoard.Properties.Resources.riqAltLeft; riq_alt_left.Size = new Size(riq_alt_left.Image.Width, riq_alt_left.Image.Height); riq_alt_left.Location = new Point(84 - minusW, 167 - minusH); Controls.Add(riq_alt_left); riq_alt_left.Hide();
            riq_alt_right = new PictureBox(); riq_alt_right.Image = RIQBoard.Properties.Resources.riqAltRight; riq_alt_right.Size = new Size(riq_alt_right.Image.Width, riq_alt_right.Image.Height); riq_alt_right.Location = new Point(281 - minusW, 167 - minusH); Controls.Add(riq_alt_right); riq_alt_right.Hide();
            riq_apostrophe = new PictureBox(); riq_apostrophe.Image = RIQBoard.Properties.Resources.riqApostrophe; riq_apostrophe.Size = new Size(riq_apostrophe.Image.Width, riq_apostrophe.Image.Height); riq_apostrophe.Location = new Point(326 - minusW, 115 - minusH); Controls.Add(riq_apostrophe); riq_apostrophe.Hide();
            riq_b = new PictureBox(); riq_b.Image = RIQBoard.Properties.Resources.riqB; riq_b.Size = new Size(riq_b.Image.Width, riq_b.Image.Height); riq_b.Location = new Point(183 - minusW, 141 - minusH); Controls.Add(riq_b); riq_b.Hide();
            riq_backslash = new PictureBox(); riq_backslash.Image = RIQBoard.Properties.Resources.riqBackslash; riq_backslash.Size = new Size(riq_backslash.Image.Width, riq_backslash.Image.Height); riq_backslash.Location = new Point(372 - minusW, 89 - minusH); Controls.Add(riq_backslash); riq_backslash.Hide();
            riq_backspace = new PictureBox(); riq_backspace.Image = RIQBoard.Properties.Resources.riqBackspace; riq_backspace.Size = new Size(riq_backspace.Image.Width, riq_backspace.Image.Height); riq_backspace.Location = new Point(359 - minusW, 62 - minusH); Controls.Add(riq_backspace); riq_backspace.Hide();
            riq_bracket_left = new PictureBox(); riq_bracket_left.Image = RIQBoard.Properties.Resources.riqBracketLeft; riq_bracket_left.Size = new Size(riq_bracket_left.Image.Width, riq_bracket_left.Image.Height); riq_bracket_left.Location = new Point(320 - minusW, 89 - minusH); Controls.Add(riq_bracket_left); riq_bracket_left.Hide();
            riq_bracket_right = new PictureBox(); riq_bracket_right.Image = RIQBoard.Properties.Resources.riqBracketRight; riq_bracket_right.Size = new Size(riq_bracket_right.Image.Width, riq_bracket_right.Image.Height); riq_bracket_right.Location = new Point(346 - minusW, 89 - minusH); Controls.Add(riq_bracket_right); riq_bracket_right.Hide();
            riq_c = new PictureBox(); riq_c.Image = RIQBoard.Properties.Resources.riqC; riq_c.Size = new Size(riq_c.Image.Width, riq_c.Image.Height); riq_c.Location = new Point(130 - minusW, 141 - minusH); Controls.Add(riq_c); riq_c.Hide();
            riq_caps_lock = new PictureBox(); riq_caps_lock.Image = RIQBoard.Properties.Resources.riqCapsLock; riq_caps_lock.Size = new Size(riq_caps_lock.Image.Width, riq_caps_lock.Image.Height); riq_caps_lock.Location = new Point(18 - minusW, 115 - minusH); Controls.Add(riq_caps_lock); riq_caps_lock.Hide();
            riq_comma = new PictureBox(); riq_comma.Image = RIQBoard.Properties.Resources.riqComma; riq_comma.Size = new Size(riq_comma.Image.Width, riq_comma.Image.Height); riq_comma.Location = new Point(261 - minusW, 141 - minusH); Controls.Add(riq_comma); riq_comma.Hide();
            riq_control_left = new PictureBox(); riq_control_left.Image = RIQBoard.Properties.Resources.riqControlLeft; riq_control_left.Size = new Size(riq_control_left.Image.Width, riq_control_left.Image.Height); riq_control_left.Location = new Point(17 - minusW, 167 - minusH); Controls.Add(riq_control_left); riq_control_left.Hide();
            riq_control_right = new PictureBox(); riq_control_right.Image = RIQBoard.Properties.Resources.riqControlRight; riq_control_right.Size = new Size(riq_control_right.Image.Width, riq_control_right.Image.Height); riq_control_right.Location = new Point(372 - minusW, 167 - minusH); Controls.Add(riq_control_right); riq_control_right.Hide();
            riq_d = new PictureBox(); riq_d.Image = RIQBoard.Properties.Resources.riqD; riq_d.Size = new Size(riq_d.Image.Width, riq_d.Image.Height); riq_d.Location = new Point(117 - minusW, 115 - minusH); Controls.Add(riq_d); riq_d.Hide();
            riq_delete = new PictureBox(); riq_delete.Image = RIQBoard.Properties.Resources.riqDelete; riq_delete.Size = new Size(riq_delete.Image.Width, riq_delete.Image.Height); riq_delete.Location = new Point(416 - minusW, 89 - minusH); Controls.Add(riq_delete); riq_delete.Hide();
            riq_dot = new PictureBox(); riq_dot.Image = RIQBoard.Properties.Resources.riqDot; riq_dot.Size = new Size(riq_dot.Image.Width, riq_dot.Image.Height); riq_dot.Location = new Point(287 - minusW, 141 - minusH); Controls.Add(riq_dot); riq_dot.Hide();
            riq_down = new PictureBox(); riq_down.Image = RIQBoard.Properties.Resources.riqDown; riq_down.Size = new Size(riq_down.Image.Width, riq_down.Image.Height); riq_down.Location = new Point(443 - minusW, 167 - minusH); Controls.Add(riq_down); riq_down.Hide();
            riq_e = new PictureBox(); riq_e.Image = RIQBoard.Properties.Resources.riqE; riq_e.Size = new Size(riq_e.Image.Width, riq_e.Image.Height); riq_e.Location = new Point(110 - minusW, 89 - minusH); Controls.Add(riq_e); riq_e.Hide();
            riq_end = new PictureBox(); riq_end.Image = RIQBoard.Properties.Resources.riqEnd; riq_end.Size = new Size(riq_end.Image.Width, riq_end.Image.Height); riq_end.Location = new Point(442 - minusW, 89 - minusH); Controls.Add(riq_end); riq_end.Hide();
            riq_enter = new PictureBox(); riq_enter.Image = RIQBoard.Properties.Resources.riqEnter; riq_enter.Size = new Size(riq_enter.Image.Width, riq_enter.Image.Height); riq_enter.Location = new Point(352 - minusW, 115 - minusH); Controls.Add(riq_enter); riq_enter.Hide();
            riq_equal = new PictureBox(); riq_equal.Image = RIQBoard.Properties.Resources.riqEqual; riq_equal.Size = new Size(riq_equal.Image.Width, riq_equal.Image.Height); riq_equal.Location = new Point(333 - minusW, 62 - minusH); Controls.Add(riq_equal); riq_equal.Hide();
            riq_escape = new PictureBox(); riq_escape.Image = RIQBoard.Properties.Resources.riqEscape; riq_escape.Size = new Size(riq_escape.Image.Width, riq_escape.Image.Height); riq_escape.Location = new Point(17 - minusW, 30 - minusH); Controls.Add(riq_escape); riq_escape.Hide();
            riq_f = new PictureBox(); riq_f.Image = RIQBoard.Properties.Resources.riqF; riq_f.Size = new Size(riq_f.Image.Width, riq_f.Image.Height); riq_f.Location = new Point(143 - minusW, 115 - minusH); Controls.Add(riq_f); riq_f.Hide();
            riq_f1 = new PictureBox(); riq_f1.Image = RIQBoard.Properties.Resources.riqF1; riq_f1.Size = new Size(riq_f1.Image.Width, riq_f1.Image.Height); riq_f1.Location = new Point(86 - minusW, 30 - minusH); Controls.Add(riq_f1); riq_f1.Hide();
            riq_f2 = new PictureBox(); riq_f2.Image = RIQBoard.Properties.Resources.riqF2; riq_f2.Size = new Size(riq_f2.Image.Width, riq_f2.Image.Height); riq_f2.Location = new Point(112 - minusW, 30 - minusH); Controls.Add(riq_f2); riq_f2.Hide();
            riq_f3 = new PictureBox(); riq_f3.Image = RIQBoard.Properties.Resources.riqF3; riq_f3.Size = new Size(riq_f3.Image.Width, riq_f3.Image.Height); riq_f3.Location = new Point(139 - minusW, 30 - minusH); Controls.Add(riq_f3); riq_f3.Hide();
            riq_f4 = new PictureBox(); riq_f4.Image = RIQBoard.Properties.Resources.riqF4; riq_f4.Size = new Size(riq_f4.Image.Width, riq_f4.Image.Height); riq_f4.Location = new Point(165 - minusW, 30 - minusH); Controls.Add(riq_f4); riq_f4.Hide();
            riq_f5 = new PictureBox(); riq_f5.Image = RIQBoard.Properties.Resources.riqF5; riq_f5.Size = new Size(riq_f5.Image.Width, riq_f5.Image.Height); riq_f5.Location = new Point(197 - minusW, 30 - minusH); Controls.Add(riq_f5); riq_f5.Hide();
            riq_f6 = new PictureBox(); riq_f6.Image = RIQBoard.Properties.Resources.riqF6; riq_f6.Size = new Size(riq_f6.Image.Width, riq_f6.Image.Height); riq_f6.Location = new Point(223 - minusW, 30 - minusH); Controls.Add(riq_f6); riq_f6.Hide();
            riq_f7 = new PictureBox(); riq_f7.Image = RIQBoard.Properties.Resources.riqF7; riq_f7.Size = new Size(riq_f7.Image.Width, riq_f7.Image.Height); riq_f7.Location = new Point(249 - minusW, 30 - minusH); Controls.Add(riq_f7); riq_f7.Hide();
            riq_f8 = new PictureBox(); riq_f8.Image = RIQBoard.Properties.Resources.riqF8; riq_f8.Size = new Size(riq_f8.Image.Width, riq_f8.Image.Height); riq_f8.Location = new Point(275 - minusW, 30 - minusH); Controls.Add(riq_f8); riq_f8.Hide();
            riq_f9 = new PictureBox(); riq_f9.Image = RIQBoard.Properties.Resources.riqF9; riq_f9.Size = new Size(riq_f9.Image.Width, riq_f9.Image.Height); riq_f9.Location = new Point(307 - minusW, 30 - minusH); Controls.Add(riq_f9); riq_f9.Hide();
            riq_f10 = new PictureBox(); riq_f10.Image = RIQBoard.Properties.Resources.riqF10; riq_f10.Size = new Size(riq_f10.Image.Width, riq_f10.Image.Height); riq_f10.Location = new Point(333 - minusW, 30 - minusH); Controls.Add(riq_f10); riq_f10.Hide();
            riq_f11 = new PictureBox(); riq_f11.Image = RIQBoard.Properties.Resources.riqF11; riq_f11.Size = new Size(riq_f11.Image.Width, riq_f11.Image.Height); riq_f11.Location = new Point(359 - minusW, 30 - minusH); Controls.Add(riq_f11); riq_f11.Hide();
            riq_f12 = new PictureBox(); riq_f12.Image = RIQBoard.Properties.Resources.riqF12; riq_f12.Size = new Size(riq_f12.Image.Width, riq_f12.Image.Height); riq_f12.Location = new Point(385 - minusW, 30 - minusH); Controls.Add(riq_f12); riq_f12.Hide();
            riq_g = new PictureBox(); riq_g.Image = RIQBoard.Properties.Resources.riqG; riq_g.Size = new Size(riq_g.Image.Width, riq_g.Image.Height); riq_g.Location = new Point(169 - minusW, 115 - minusH); Controls.Add(riq_g); riq_g.Hide();
            riq_h = new PictureBox(); riq_h.Image = RIQBoard.Properties.Resources.riqH; riq_h.Size = new Size(riq_h.Image.Width, riq_h.Image.Height); riq_h.Location = new Point(196 - minusW, 115 - minusH); Controls.Add(riq_h); riq_h.Hide();
            riq_home = new PictureBox(); riq_home.Image = RIQBoard.Properties.Resources.riqHome; riq_home.Size = new Size(riq_home.Image.Width, riq_home.Image.Height); riq_home.Location = new Point(442 - minusW, 62 - minusH); Controls.Add(riq_home); riq_home.Hide();
            riq_i = new PictureBox(); riq_i.Image = RIQBoard.Properties.Resources.riqI; riq_i.Size = new Size(riq_i.Image.Width, riq_i.Image.Height); riq_i.Location = new Point(241 - minusW, 89 - minusH); Controls.Add(riq_i); riq_i.Hide();
            riq_insert = new PictureBox(); riq_insert.Image = RIQBoard.Properties.Resources.riqInsert; riq_insert.Size = new Size(riq_insert.Image.Width, riq_insert.Image.Height); riq_insert.Location = new Point(416 - minusW, 62 - minusH); Controls.Add(riq_insert); riq_insert.Hide();
            riq_j = new PictureBox(); riq_j.Image = RIQBoard.Properties.Resources.riqJ; riq_j.Size = new Size(riq_j.Image.Width, riq_j.Image.Height); riq_j.Location = new Point(222 - minusW, 115 - minusH); Controls.Add(riq_j); riq_j.Hide();
            riq_k = new PictureBox(); riq_k.Image = RIQBoard.Properties.Resources.riqK; riq_k.Size = new Size(riq_k.Image.Width, riq_k.Image.Height); riq_k.Location = new Point(248 - minusW, 115 - minusH); Controls.Add(riq_k); riq_k.Hide();
            riq_l = new PictureBox(); riq_l.Image = RIQBoard.Properties.Resources.riqL; riq_l.Size = new Size(riq_l.Image.Width, riq_l.Image.Height); riq_l.Location = new Point(274 - minusW, 115 - minusH); Controls.Add(riq_l); riq_l.Hide();
            riq_left = new PictureBox(); riq_left.Image = RIQBoard.Properties.Resources.riqLeft; riq_left.Size = new Size(riq_left.Image.Width, riq_left.Image.Height); riq_left.Location = new Point(417 - minusW, 167 - minusH); Controls.Add(riq_left); riq_left.Hide();
            riq_list = new PictureBox(); riq_list.Image = RIQBoard.Properties.Resources.riqList; riq_list.Size = new Size(riq_list.Image.Width, riq_list.Image.Height); riq_list.Location = new Point(346 - minusW, 167 - minusH); Controls.Add(riq_list); riq_list.Hide();
            riq_m = new PictureBox(); riq_m.Image = RIQBoard.Properties.Resources.riqM; riq_m.Size = new Size(riq_m.Image.Width, riq_m.Image.Height); riq_m.Location = new Point(235 - minusW, 141 - minusH); Controls.Add(riq_m); riq_m.Hide();
            riq_minus = new PictureBox(); riq_minus.Image = RIQBoard.Properties.Resources.riqMinus; riq_minus.Size = new Size(riq_minus.Image.Width, riq_minus.Image.Height); riq_minus.Location = new Point(306 - minusW, 62 - minusH); Controls.Add(riq_minus); riq_minus.Hide();
            riq_n = new PictureBox(); riq_n.Image = RIQBoard.Properties.Resources.riqN; riq_n.Size = new Size(riq_n.Image.Width, riq_n.Image.Height); riq_n.Location = new Point(209 - minusW, 141 - minusH); Controls.Add(riq_n); riq_n.Hide();
            riq_num_lock = new PictureBox(); riq_num_lock.Image = RIQBoard.Properties.Resources.riqNumLock; riq_num_lock.Size = new Size(riq_num_lock.Image.Width, riq_num_lock.Image.Height); riq_num_lock.Location = new Point(500 - minusW, 62 - minusH); Controls.Add(riq_num_lock); riq_num_lock.Hide();
            riq_num_lock_0 = new PictureBox(); riq_num_lock_0.Image = RIQBoard.Properties.Resources.riqNumLock0; riq_num_lock_0.Size = new Size(riq_num_lock_0.Image.Width, riq_num_lock_0.Image.Height); riq_num_lock_0.Location = new Point(501 - minusW, 167 - minusH); Controls.Add(riq_num_lock_0); riq_num_lock_0.Hide();
            riq_num_lock_1 = new PictureBox(); riq_num_lock_1.Image = RIQBoard.Properties.Resources.riqNumLock1; riq_num_lock_1.Size = new Size(riq_num_lock_1.Image.Width, riq_num_lock_1.Image.Height); riq_num_lock_1.Location = new Point(500 - minusW, 141 - minusH); Controls.Add(riq_num_lock_1); riq_num_lock_1.Hide();
            riq_num_lock_2 = new PictureBox(); riq_num_lock_2.Image = RIQBoard.Properties.Resources.riqNumLock2; riq_num_lock_2.Size = new Size(riq_num_lock_2.Image.Width, riq_num_lock_2.Image.Height); riq_num_lock_2.Location = new Point(526 - minusW, 141 - minusH); Controls.Add(riq_num_lock_2); riq_num_lock_2.Hide();
            riq_num_lock_3 = new PictureBox(); riq_num_lock_3.Image = RIQBoard.Properties.Resources.riqNumLock3; riq_num_lock_3.Size = new Size(riq_num_lock_3.Image.Width, riq_num_lock_3.Image.Height); riq_num_lock_3.Location = new Point(552 - minusW, 141 - minusH); Controls.Add(riq_num_lock_3); riq_num_lock_3.Hide();
            riq_num_lock_4 = new PictureBox(); riq_num_lock_4.Image = RIQBoard.Properties.Resources.riqNumLock4; riq_num_lock_4.Size = new Size(riq_num_lock_4.Image.Width, riq_num_lock_4.Image.Height); riq_num_lock_4.Location = new Point(500 - minusW, 115 - minusH); Controls.Add(riq_num_lock_4); riq_num_lock_4.Hide();
            riq_num_lock_5 = new PictureBox(); riq_num_lock_5.Image = RIQBoard.Properties.Resources.riqNumLock5; riq_num_lock_5.Size = new Size(riq_num_lock_5.Image.Width, riq_num_lock_5.Image.Height); riq_num_lock_5.Location = new Point(526 - minusW, 115 - minusH); Controls.Add(riq_num_lock_5); riq_num_lock_5.Hide();
            riq_num_lock_6 = new PictureBox(); riq_num_lock_6.Image = RIQBoard.Properties.Resources.riqNumLock6; riq_num_lock_6.Size = new Size(riq_num_lock_6.Image.Width, riq_num_lock_6.Image.Height); riq_num_lock_6.Location = new Point(551 - minusW, 115 - minusH); Controls.Add(riq_num_lock_6); riq_num_lock_6.Hide();
            riq_num_lock_7 = new PictureBox(); riq_num_lock_7.Image = RIQBoard.Properties.Resources.riqNumLock7; riq_num_lock_7.Size = new Size(riq_num_lock_7.Image.Width, riq_num_lock_7.Image.Height); riq_num_lock_7.Location = new Point(500 - minusW, 89 - minusH); Controls.Add(riq_num_lock_7); riq_num_lock_7.Hide();
            riq_num_lock_8 = new PictureBox(); riq_num_lock_8.Image = RIQBoard.Properties.Resources.riqNumLock8; riq_num_lock_8.Size = new Size(riq_num_lock_8.Image.Width, riq_num_lock_8.Image.Height); riq_num_lock_8.Location = new Point(525 - minusW, 89 - minusH); Controls.Add(riq_num_lock_8); riq_num_lock_8.Hide();
            riq_num_lock_9 = new PictureBox(); riq_num_lock_9.Image = RIQBoard.Properties.Resources.riqNumLock9; riq_num_lock_9.Size = new Size(riq_num_lock_9.Image.Width, riq_num_lock_9.Image.Height); riq_num_lock_9.Location = new Point(551 - minusW, 89 - minusH); Controls.Add(riq_num_lock_9); riq_num_lock_9.Hide();
            riq_num_lock_asterisk = new PictureBox(); riq_num_lock_asterisk.Image = RIQBoard.Properties.Resources.riqNumLockAsterisk; riq_num_lock_asterisk.Size = new Size(riq_num_lock_asterisk.Image.Width, riq_num_lock_asterisk.Image.Height); riq_num_lock_asterisk.Location = new Point(551 - minusW, 62 - minusH); Controls.Add(riq_num_lock_asterisk); riq_num_lock_asterisk.Hide();
            riq_num_lock_dot = new PictureBox(); riq_num_lock_dot.Image = RIQBoard.Properties.Resources.riqNumLockDot; riq_num_lock_dot.Size = new Size(riq_num_lock_dot.Image.Width, riq_num_lock_dot.Image.Height); riq_num_lock_dot.Location = new Point(551 - minusW, 167 - minusH); Controls.Add(riq_num_lock_dot); riq_num_lock_dot.Hide();
            riq_num_lock_enter = new PictureBox(); riq_num_lock_enter.Image = RIQBoard.Properties.Resources.riqNumLockEnter; riq_num_lock_enter.Size = new Size(riq_num_lock_enter.Image.Width, riq_num_lock_enter.Image.Height); riq_num_lock_enter.Location = new Point(578 - minusW, 141 - minusH); Controls.Add(riq_num_lock_enter); riq_num_lock_enter.Hide();
            riq_num_lock_minus = new PictureBox(); riq_num_lock_minus.Image = RIQBoard.Properties.Resources.riqNumLockMinus; riq_num_lock_minus.Size = new Size(riq_num_lock_minus.Image.Width, riq_num_lock_minus.Image.Height); riq_num_lock_minus.Location = new Point(577 - minusW, 62 - minusH); Controls.Add(riq_num_lock_minus); riq_num_lock_minus.Hide();
            riq_num_lock_plus = new PictureBox(); riq_num_lock_plus.Image = RIQBoard.Properties.Resources.riqNumLockPlus; riq_num_lock_plus.Size = new Size(riq_num_lock_plus.Image.Width, riq_num_lock_plus.Image.Height); riq_num_lock_plus.Location = new Point(577 - minusW, 89 - minusH); Controls.Add(riq_num_lock_plus); riq_num_lock_plus.Hide();
            riq_num_lock_slash = new PictureBox(); riq_num_lock_slash.Image = RIQBoard.Properties.Resources.riqNumLockSlash; riq_num_lock_slash.Size = new Size(riq_num_lock_slash.Image.Width, riq_num_lock_slash.Image.Height); riq_num_lock_slash.Location = new Point(525 - minusW, 62 - minusH); Controls.Add(riq_num_lock_slash); riq_num_lock_slash.Hide();
            riq_o = new PictureBox(); riq_o.Image = RIQBoard.Properties.Resources.riqO; riq_o.Size = new Size(riq_o.Image.Width, riq_o.Image.Height); riq_o.Location = new Point(268 - minusW, 89 - minusH); Controls.Add(riq_o); riq_o.Hide();
            riq_oemtilde = new PictureBox(); riq_oemtilde.Image = RIQBoard.Properties.Resources.riqOemtilde; riq_oemtilde.Size = new Size(riq_oemtilde.Image.Width, riq_oemtilde.Image.Height); riq_oemtilde.Location = new Point(18 - minusW, 62 - minusH); Controls.Add(riq_oemtilde); riq_oemtilde.Hide();
            riq_p = new PictureBox(); riq_p.Image = RIQBoard.Properties.Resources.riqP; riq_p.Size = new Size(riq_p.Image.Width, riq_p.Image.Height); riq_p.Location = new Point(294 - minusW, 89 - minusH); Controls.Add(riq_p); riq_p.Hide();
            riq_page_down = new PictureBox(); riq_page_down.Image = RIQBoard.Properties.Resources.riqPageDown; riq_page_down.Size = new Size(riq_page_down.Image.Width, riq_page_down.Image.Height); riq_page_down.Location = new Point(468 - minusW, 89 - minusH); Controls.Add(riq_page_down); riq_page_down.Hide();
            riq_page_up = new PictureBox(); riq_page_up.Image = RIQBoard.Properties.Resources.riqPageUp; riq_page_up.Size = new Size(riq_page_up.Image.Width, riq_page_up.Image.Height); riq_page_up.Location = new Point(468 - minusW, 62 - minusH); Controls.Add(riq_page_up); riq_page_up.Hide();
            riq_pause = new PictureBox(); riq_pause.Image = RIQBoard.Properties.Resources.riqPause; riq_pause.Size = new Size(riq_pause.Image.Width, riq_pause.Image.Height); riq_pause.Location = new Point(468 - minusW, 30 - minusH); Controls.Add(riq_pause); riq_pause.Hide();
            riq_print_screen = new PictureBox(); riq_print_screen.Image = RIQBoard.Properties.Resources.riqPrintScreen; riq_print_screen.Size = new Size(riq_print_screen.Image.Width, riq_print_screen.Image.Height); riq_print_screen.Location = new Point(416 - minusW, 30 - minusH); Controls.Add(riq_print_screen); riq_print_screen.Hide();
            riq_q = new PictureBox(); riq_q.Image = RIQBoard.Properties.Resources.riqQ; riq_q.Size = new Size(riq_q.Image.Width, riq_q.Image.Height); riq_q.Location = new Point(58 - minusW, 89 - minusH); Controls.Add(riq_q); riq_q.Hide();
            riq_r = new PictureBox(); riq_r.Image = RIQBoard.Properties.Resources.riqR; riq_r.Size = new Size(riq_r.Image.Width, riq_r.Image.Height); riq_r.Location = new Point(137 - minusW, 89 - minusH); Controls.Add(riq_r); riq_r.Hide();
            riq_right = new PictureBox(); riq_right.Image = RIQBoard.Properties.Resources.riqRight; riq_right.Size = new Size(riq_right.Image.Width, riq_right.Image.Height); riq_right.Location = new Point(469 - minusW, 167 - minusH); Controls.Add(riq_right); riq_right.Hide();
            riq_s = new PictureBox(); riq_s.Image = RIQBoard.Properties.Resources.riqS; riq_s.Size = new Size(riq_s.Image.Width, riq_s.Image.Height); riq_s.Location = new Point(90 - minusW, 115 - minusH); Controls.Add(riq_s); riq_s.Hide();
            riq_scroll_lock = new PictureBox(); riq_scroll_lock.Image = RIQBoard.Properties.Resources.riqScrollLock; riq_scroll_lock.Size = new Size(riq_scroll_lock.Image.Width, riq_scroll_lock.Image.Height); riq_scroll_lock.Location = new Point(442 - minusW, 30 - minusH); Controls.Add(riq_scroll_lock); riq_scroll_lock.Hide();
            riq_semicolon = new PictureBox(); riq_semicolon.Image = RIQBoard.Properties.Resources.riqSemicolon; riq_semicolon.Size = new Size(riq_semicolon.Image.Width, riq_semicolon.Image.Height); riq_semicolon.Location = new Point(300 - minusW, 115 - minusH); Controls.Add(riq_semicolon); riq_semicolon.Hide();
            riq_shift_left = new PictureBox(); riq_shift_left.Image = RIQBoard.Properties.Resources.riqShiftLeft; riq_shift_left.Size = new Size(riq_shift_left.Image.Width, riq_shift_left.Image.Height); riq_shift_left.Location = new Point(18 - minusW, 141 - minusH); Controls.Add(riq_shift_left); riq_shift_left.Hide();
            riq_shift_right = new PictureBox(); riq_shift_right.Image = RIQBoard.Properties.Resources.riqShiftRight; riq_shift_right.Size = new Size(riq_shift_right.Image.Width, riq_shift_right.Image.Height); riq_shift_right.Location = new Point(339 - minusW, 141 - minusH); Controls.Add(riq_shift_right); riq_shift_right.Hide();
            riq_slash = new PictureBox(); riq_slash.Image = RIQBoard.Properties.Resources.riqSlash; riq_slash.Size = new Size(riq_slash.Image.Width, riq_slash.Image.Height); riq_slash.Location = new Point(313 - minusW, 141 - minusH); Controls.Add(riq_slash); riq_slash.Hide();
            riq_space = new PictureBox(); riq_space.Image = RIQBoard.Properties.Resources.riqSpace; riq_space.Size = new Size(riq_space.Image.Width, riq_space.Image.Height); riq_space.Location = new Point(123 - minusW, 167 - minusH); Controls.Add(riq_space); riq_space.Hide();
            riq_t = new PictureBox(); riq_t.Image = RIQBoard.Properties.Resources.riqT; riq_t.Size = new Size(riq_t.Image.Width, riq_t.Image.Height); riq_t.Location = new Point(163 - minusW, 89 - minusH); Controls.Add(riq_t); riq_t.Hide();
            riq_tab = new PictureBox(); riq_tab.Image = RIQBoard.Properties.Resources.riqTab; riq_tab.Size = new Size(riq_tab.Image.Width, riq_tab.Image.Height); riq_tab.Location = new Point(18 - minusW, 89 - minusH); Controls.Add(riq_tab); riq_tab.Hide();
            riq_u = new PictureBox(); riq_u.Image = RIQBoard.Properties.Resources.riqU; riq_u.Size = new Size(riq_u.Image.Width, riq_u.Image.Height); riq_u.Location = new Point(215 - minusW, 89 - minusH); Controls.Add(riq_u); riq_u.Hide();
            riq_up = new PictureBox(); riq_up.Image = RIQBoard.Properties.Resources.riqUp; riq_up.Size = new Size(riq_up.Image.Width, riq_up.Image.Height); riq_up.Location = new Point(443 - minusW, 141 - minusH); Controls.Add(riq_up); riq_up.Hide();
            riq_v = new PictureBox(); riq_v.Image = RIQBoard.Properties.Resources.riqV; riq_v.Size = new Size(riq_v.Image.Width, riq_v.Image.Height); riq_v.Location = new Point(156 - minusW, 141 - minusH); Controls.Add(riq_v); riq_v.Hide();
            riq_w = new PictureBox(); riq_w.Image = RIQBoard.Properties.Resources.riqW; riq_w.Size = new Size(riq_w.Image.Width, riq_w.Image.Height); riq_w.Location = new Point(84 - minusW, 89 - minusH); Controls.Add(riq_w); riq_w.Hide();
            riq_windows_left = new PictureBox(); riq_windows_left.Image = RIQBoard.Properties.Resources.riqWindowsLeft; riq_windows_left.Size = new Size(riq_windows_left.Image.Width, riq_windows_left.Image.Height); riq_windows_left.Location = new Point(57 - minusW, 167 - minusH); Controls.Add(riq_windows_left); riq_windows_left.Hide();
            riq_windows_right = new PictureBox(); riq_windows_right.Image = RIQBoard.Properties.Resources.riqWindowsRight; riq_windows_right.Size = new Size(riq_windows_right.Image.Width, riq_windows_right.Image.Height); riq_windows_right.Location = new Point(320 - minusW, 167 - minusH); Controls.Add(riq_windows_right); riq_windows_right.Hide();
            riq_x = new PictureBox(); riq_x.Image = RIQBoard.Properties.Resources.riqX; riq_x.Size = new Size(riq_x.Image.Width, riq_x.Image.Height); riq_x.Location = new Point(104 - minusW, 141 - minusH); Controls.Add(riq_x); riq_x.Hide();
            riq_y = new PictureBox(); riq_y.Image = RIQBoard.Properties.Resources.riqY; riq_y.Size = new Size(riq_y.Image.Width, riq_y.Image.Height); riq_y.Location = new Point(189 - minusW, 89 - minusH); Controls.Add(riq_y); riq_y.Hide();
            riq_z = new PictureBox(); riq_z.Image = RIQBoard.Properties.Resources.riqZ; riq_z.Size = new Size(riq_z.Image.Width, riq_z.Image.Height); riq_z.Location = new Point(77 - minusW, 141 - minusH); Controls.Add(riq_z); riq_z.Hide();
            riq_layout = new PictureBox(); riq_layout.Image = RIQBoard.Properties.Resources.riqboardLayout; riq_layout.Size = new Size(riq_layout.Image.Width, riq_layout.Image.Height); riq_layout.Location = new Point(0, 0); Controls.Add(riq_layout); riq_layout.Hide();
            riq_update = new PictureBox(); riq_update.Image = RIQBoard.Properties.Resources.riqboardUpdate; riq_update.Size = new Size(riq_update.Image.Width, riq_update.Image.Height); riq_update.Location = new Point(0, 0); Controls.Add(riq_update); riq_update.Hide();
            riq = new PictureBox(); riq.Image = RIQBoard.Properties.Resources.riqboard; riq.Size = new Size(riq.Image.Width, riq.Image.Height); riq.Location = new Point(0, 0); Controls.Add(riq);
            #endregion
            #region labes
            label_1 = new Label(); label_1.Font = font; label_1.ForeColor = color; label_1.Size = new Size(16, 32); label_1.Location = new Point(520, 1); label_1.BackColor = Color.Transparent; Controls.Add(label_1); label_1.Parent = riq; label_1.Text = "0";
            label_2 = new Label(); label_2.Font = font; label_2.ForeColor = color; label_2.Size = new Size(16, 32); label_2.Location = new Point(531, 1); label_2.BackColor = Color.Transparent; Controls.Add(label_2); label_2.Parent = riq; label_2.Text = "0";
            label_3 = new Label(); label_3.Font = font; label_3.ForeColor = color; label_3.Size = new Size(16, 32); label_3.Location = new Point(543, 1); label_3.BackColor = Color.Transparent; Controls.Add(label_3); label_3.Parent = riq; label_3.Text = "0";
            #endregion
            UpdateChecker();
        }
        /* dublicates:
        Enter, Return
        CapsLock, Capital
        HangulMode, HanguelMode, KanaMode
        KanjiMode, HanjaMode
        IMEAccept, IMEAceept
        Prior, PageUp
        PageDown, Next
        Snapshot, PrintScreen
        OemSemicolon, Oem1
        Oem2, OemQuestion
        Oem3, Oemtilde
        Oem4, OemOpenBrackets
        OemPipe, Oem5
        OemCloseBrackets, Oem6
        OemQuotes, Oem7
        Oem102, OemBackslash
        */
        private void Apm()
        {
            if (apmBool == false)
            {
                apmBool = true;
                apmPlus = 0;
                firstSec = DateTime.Now.Second;
                currMin = DateTime.Now.Minute;
                if (firstSec < 61 - period) { lastSec = firstSec + period; }
                else { lastSec = firstSec - (60 - period); }
            }
            if (apmBool == true)
            {
                apmPlus++;
                currSec = DateTime.Now.Second;
                if (DateTime.Now.Minute == currMin)
                {
                    if (currSec == lastSec)
                    {
                        apmBool = false;
                        apm = apmPlus * (60 / period);
                        CatchRange();
                    }
                    else if (currSec > lastSec || currSec < firstSec)
                    {
                        apmBool = false;
                        label_1.Text = "0";
                        label_2.Text = "0";
                        label_3.Text = "0";
                    }
                }
                else { apmBool = false; }
            }
        }
        private void CatchRange()
        {
            if (apm < 100) { ConverterTwo(); }
            else if (apm >= 100 && apm < 999) { ConverterThree(); }
            else if (apm >= 999)
            {
                label_1.Text = "9";
                label_2.Text = "9";
                label_3.Text = "9";
            }
        }
        private void ConverterTwo()
        {
            label_3.Hide();
            apmString = apm.ToString();
            apmString.ToCharArray();
            if (apmString[0].ToString() == "0") { label_1.Text = "0"; }
            else if (apmString[0].ToString() == "1") { label_1.Text = "1"; }
            else if (apmString[0].ToString() == "2") { label_1.Text = "2"; }
            else if (apmString[0].ToString() == "3") { label_1.Text = "3"; }
            else if (apmString[0].ToString() == "4") { label_1.Text = "4"; }
            else if (apmString[0].ToString() == "5") { label_1.Text = "5"; }
            else if (apmString[0].ToString() == "6") { label_1.Text = "6"; }
            else if (apmString[0].ToString() == "7") { label_1.Text = "7"; }
            else if (apmString[0].ToString() == "8") { label_1.Text = "8"; }
            else if (apmString[0].ToString() == "9") { label_1.Text = "9"; }

            if (apmString[1].ToString() == "0") { label_2.Text = "0"; }
            else if (apmString[1].ToString() == "1") { label_2.Text = "1"; }
            else if (apmString[1].ToString() == "2") { label_2.Text = "2"; }
            else if (apmString[1].ToString() == "3") { label_2.Text = "3"; }
            else if (apmString[1].ToString() == "4") { label_2.Text = "4"; }
            else if (apmString[1].ToString() == "5") { label_2.Text = "5"; }
            else if (apmString[1].ToString() == "6") { label_2.Text = "6"; }
            else if (apmString[1].ToString() == "7") { label_2.Text = "7"; }
            else if (apmString[1].ToString() == "8") { label_2.Text = "8"; }
            else if (apmString[1].ToString() == "9") { label_2.Text = "9"; }
        }
        private void ConverterThree()
        {
            label_3.Show();
            apmString = apm.ToString();
            apmString.ToCharArray();
            if (apmString[0].ToString() == "0") { label_1.Text = "0"; }
            else if (apmString[0].ToString() == "1") { label_1.Text = "1"; }
            else if (apmString[0].ToString() == "2") { label_1.Text = "2"; }
            else if (apmString[0].ToString() == "3") { label_1.Text = "3"; }
            else if (apmString[0].ToString() == "4") { label_1.Text = "4"; }
            else if (apmString[0].ToString() == "5") { label_1.Text = "5"; }
            else if (apmString[0].ToString() == "6") { label_1.Text = "6"; }
            else if (apmString[0].ToString() == "7") { label_1.Text = "7"; }
            else if (apmString[0].ToString() == "8") { label_1.Text = "8"; }
            else if (apmString[0].ToString() == "9") { label_1.Text = "9"; }

            if (apmString[1].ToString() == "0") { label_2.Text = "0"; }
            else if (apmString[1].ToString() == "1") { label_2.Text = "1"; }
            else if (apmString[1].ToString() == "2") { label_2.Text = "2"; }
            else if (apmString[1].ToString() == "3") { label_2.Text = "3"; }
            else if (apmString[1].ToString() == "4") { label_2.Text = "4"; }
            else if (apmString[1].ToString() == "5") { label_2.Text = "5"; }
            else if (apmString[1].ToString() == "6") { label_2.Text = "6"; }
            else if (apmString[1].ToString() == "7") { label_2.Text = "7"; }
            else if (apmString[1].ToString() == "8") { label_2.Text = "8"; }
            else if (apmString[1].ToString() == "9") { label_2.Text = "9"; }

            if (apmString[2].ToString() == "0") { label_3.Text = "0"; }
            else if (apmString[2].ToString() == "1") { label_3.Text = "1"; }
            else if (apmString[2].ToString() == "2") { label_3.Text = "2"; }
            else if (apmString[2].ToString() == "3") { label_3.Text = "3"; }
            else if (apmString[2].ToString() == "4") { label_3.Text = "4"; }
            else if (apmString[2].ToString() == "5") { label_3.Text = "5"; }
            else if (apmString[2].ToString() == "6") { label_3.Text = "6"; }
            else if (apmString[2].ToString() == "7") { label_3.Text = "7"; }
            else if (apmString[2].ToString() == "8") { label_3.Text = "8"; }
            else if (apmString[2].ToString() == "9") { label_3.Text = "9"; }
        }
        private void UpdateChecker()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.googledrive.com/host/0B_S90MIGo2ICbGRjZ2tSV09GWVU");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader stream = new StreamReader(response.GetResponseStream());
            string status = stream.ReadToEnd();
            if (status != version) { riq_update.Show(); }
        }
        private void Up(object sender, KeyEventArgs e)
        {
            Apm();
            if (e.KeyCode == Keys.D0) { riq_0.Hide(); }
            if (e.KeyCode == Keys.D1) { riq_1.Hide(); }
            if (e.KeyCode == Keys.D2) { riq_2.Hide(); }
            if (e.KeyCode == Keys.D3) { riq_3.Hide(); }
            if (e.KeyCode == Keys.D4) { riq_4.Hide(); }
            if (e.KeyCode == Keys.D5) { riq_5.Hide(); }
            if (e.KeyCode == Keys.D6) { riq_6.Hide(); }
            if (e.KeyCode == Keys.D7) { riq_7.Hide(); }
            if (e.KeyCode == Keys.D8) { riq_8.Hide(); }
            if (e.KeyCode == Keys.D9) { riq_9.Hide(); }
            if (e.KeyCode == Keys.A) { riq_a.Hide(); }
            if (e.KeyCode == Keys.LMenu) { riq_alt_left.Hide(); }
            if (e.KeyCode == Keys.RMenu) { riq_alt_right.Hide(); }
            if (e.KeyCode == Keys.OemQuotes) { riq_apostrophe.Hide(); }
            if (e.KeyCode == Keys.B) { riq_b.Hide(); }
            if (e.KeyCode == Keys.Oem5) { riq_backslash.Hide(); }
            if (e.KeyCode == Keys.Back) { riq_backspace.Hide(); }
            if (e.KeyCode == Keys.OemOpenBrackets) { riq_bracket_left.Hide(); }
            if (e.KeyCode == Keys.OemCloseBrackets) { riq_bracket_right.Hide(); }
            if (e.KeyCode == Keys.C) { riq_c.Hide(); }
            if (e.KeyCode == Keys.CapsLock) { riq_caps_lock.Hide(); }
            if (e.KeyCode == Keys.Oemcomma) { riq_comma.Hide(); }
            if (e.KeyCode == Keys.LControlKey) { riq_control_left.Hide(); }
            if (e.KeyCode == Keys.RControlKey) { riq_control_right.Hide(); }
            if (e.KeyCode == Keys.D) { riq_d.Hide(); }
            if (e.KeyCode == Keys.Delete) { riq_delete.Hide(); }
            if (e.KeyCode == Keys.OemPeriod) { riq_dot.Hide(); }
            if (e.KeyCode == Keys.Down) { riq_down.Hide(); }
            if (e.KeyCode == Keys.E) { riq_e.Hide(); }
            if (e.KeyCode == Keys.End) { riq_end.Hide(); }
            if (e.KeyCode == Keys.Enter) { riq_enter.Hide(); }
            if (e.KeyCode == Keys.Oemplus) { riq_equal.Hide(); }
            if (e.KeyCode == Keys.Escape) { riq_escape.Hide(); }
            if (e.KeyCode == Keys.F) { riq_f.Hide(); }
            if (e.KeyCode == Keys.F1) { riq_f1.Hide(); }
            if (e.KeyCode == Keys.F2) { riq_f2.Hide(); }
            if (e.KeyCode == Keys.F3) { riq_f3.Hide(); }
            if (e.KeyCode == Keys.F4) { riq_f4.Hide(); }
            if (e.KeyCode == Keys.F5) { riq_f5.Hide(); }
            if (e.KeyCode == Keys.F6) { riq_f6.Hide(); }
            if (e.KeyCode == Keys.F7) { riq_f7.Hide(); }
            if (e.KeyCode == Keys.F8) { riq_f8.Hide(); }
            if (e.KeyCode == Keys.F9) { riq_f9.Hide(); }
            if (e.KeyCode == Keys.F10) { riq_f10.Hide(); }
            if (e.KeyCode == Keys.F11) { riq_f11.Hide(); }
            if (e.KeyCode == Keys.F12) { riq_f12.Hide(); }
            if (e.KeyCode == Keys.G) { riq_g.Hide(); }
            if (e.KeyCode == Keys.H) { riq_h.Hide(); }
            if (e.KeyCode == Keys.Home) { riq_home.Hide(); }
            if (e.KeyCode == Keys.I) { riq_i.Hide(); }
            if (e.KeyCode == Keys.Insert) { riq_insert.Hide(); }
            if (e.KeyCode == Keys.J) { riq_j.Hide(); }
            if (e.KeyCode == Keys.K) { riq_k.Hide(); }
            if (e.KeyCode == Keys.L) { riq_l.Hide(); }
            if (e.KeyCode == Keys.Left) { riq_left.Hide(); }
            if (e.KeyCode == Keys.Menu) { riq_list.Hide(); }
            if (e.KeyCode == Keys.M) { riq_m.Hide(); }
            if (e.KeyCode == Keys.OemMinus) { riq_minus.Hide(); }
            if (e.KeyCode == Keys.N) { riq_n.Hide(); }
            if (e.KeyCode == Keys.NumLock) { riq_num_lock.Hide(); }
            if (e.KeyCode == Keys.NumPad0) { riq_num_lock_0.Hide(); }
            if (e.KeyCode == Keys.NumPad1) { riq_num_lock_1.Hide(); }
            if (e.KeyCode == Keys.NumPad2) { riq_num_lock_2.Hide(); }
            if (e.KeyCode == Keys.NumPad3) { riq_num_lock_3.Hide(); }
            if (e.KeyCode == Keys.NumPad4) { riq_num_lock_4.Hide(); }
            if (e.KeyCode == Keys.NumPad5) { riq_num_lock_5.Hide(); }
            if (e.KeyCode == Keys.NumPad6) { riq_num_lock_6.Hide(); }
            if (e.KeyCode == Keys.NumPad7) { riq_num_lock_7.Hide(); }
            if (e.KeyCode == Keys.NumPad8) { riq_num_lock_8.Hide(); }
            if (e.KeyCode == Keys.NumPad9) { riq_num_lock_9.Hide(); }
            if (e.KeyCode == Keys.Multiply) { riq_num_lock_asterisk.Hide(); }
            if (e.KeyCode == Keys.Decimal) { riq_num_lock_dot.Hide(); }
            if (e.KeyCode == Keys.Return) { riq_num_lock_enter.Hide(); }
            if (e.KeyCode == Keys.Subtract) { riq_num_lock_minus.Hide(); }
            if (e.KeyCode == Keys.Add) { riq_num_lock_plus.Hide(); }
            if (e.KeyCode == Keys.Divide) { riq_num_lock_slash.Hide(); }
            if (e.KeyCode == Keys.O) { riq_o.Hide(); }
            if (e.KeyCode == Keys.Oemtilde) { riq_oemtilde.Hide(); }
            if (e.KeyCode == Keys.P) { riq_p.Hide(); }
            if (e.KeyCode == Keys.PageDown) { riq_page_down.Hide(); }
            if (e.KeyCode == Keys.PageUp) { riq_page_up.Hide(); }
            if (e.KeyCode == Keys.Pause) { riq_pause.Hide(); }
            if (e.KeyCode == Keys.PrintScreen) { riq_print_screen.Hide(); }
            if (e.KeyCode == Keys.Q) { riq_q.Hide(); }
            if (e.KeyCode == Keys.R) { riq_r.Hide(); }
            if (e.KeyCode == Keys.Right) { riq_right.Hide(); }
            if (e.KeyCode == Keys.S) { riq_s.Hide(); }
            if (e.KeyCode == Keys.Scroll) { riq_scroll_lock.Hide(); }
            if (e.KeyCode == Keys.OemSemicolon) { riq_semicolon.Hide(); }
            if (e.KeyCode == Keys.LShiftKey) { riq_shift_left.Hide(); }
            if (e.KeyCode == Keys.RShiftKey) { riq_shift_right.Hide(); }
            if (e.KeyCode == Keys.OemQuestion) { riq_slash.Hide(); }
            if (e.KeyCode == Keys.Space) { riq_space.Hide(); }
            if (e.KeyCode == Keys.T) { riq_t.Hide(); }
            if (e.KeyCode == Keys.Tab) { riq_tab.Hide(); }
            if (e.KeyCode == Keys.U) { riq_u.Hide(); }
            if (e.KeyCode == Keys.Up) { riq_up.Hide(); }
            if (e.KeyCode == Keys.V) { riq_v.Hide(); }
            if (e.KeyCode == Keys.W) { riq_w.Hide(); }
            if (e.KeyCode == Keys.LWin) { riq_windows_left.Hide(); }
            if (e.KeyCode == Keys.RWin) { riq_windows_right.Hide(); }
            if (e.KeyCode == Keys.X) { riq_x.Hide(); }
            if (e.KeyCode == Keys.Y) { riq_y.Hide(); }
            if (e.KeyCode == Keys.Z) { riq_z.Hide(); }
        }
        private void Down(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D0) { riq_0.Show(); }
            if (e.KeyCode == Keys.D1) { riq_1.Show(); }
            if (e.KeyCode == Keys.D2) { riq_2.Show(); }
            if (e.KeyCode == Keys.D3) { riq_3.Show(); }
            if (e.KeyCode == Keys.D4) { riq_4.Show(); }
            if (e.KeyCode == Keys.D5) { riq_5.Show(); }
            if (e.KeyCode == Keys.D6) { riq_6.Show(); }
            if (e.KeyCode == Keys.D7) { riq_7.Show(); }
            if (e.KeyCode == Keys.D8) { riq_8.Show(); }
            if (e.KeyCode == Keys.D9) { riq_9.Show(); }
            if (e.KeyCode == Keys.A) { riq_a.Show(); }
            if (e.KeyCode == Keys.LMenu) { riq_alt_left.Show(); }
            if (e.KeyCode == Keys.RMenu) { riq_alt_right.Show(); }
            if (e.KeyCode == Keys.OemQuotes) { riq_apostrophe.Show(); }
            if (e.KeyCode == Keys.B) { riq_b.Show(); }
            if (e.KeyCode == Keys.Oem5) { riq_backslash.Show(); }
            if (e.KeyCode == Keys.Back) { riq_backspace.Show(); }
            if (e.KeyCode == Keys.OemOpenBrackets) { riq_bracket_left.Show(); }
            if (e.KeyCode == Keys.OemCloseBrackets) { riq_bracket_right.Show(); }
            if (e.KeyCode == Keys.C) { riq_c.Show(); }
            if (e.KeyCode == Keys.CapsLock) { riq_caps_lock.Show(); }
            if (e.KeyCode == Keys.Oemcomma) { riq_comma.Show(); }
            if (e.KeyCode == Keys.LControlKey) { riq_control_left.Show(); }
            if (e.KeyCode == Keys.RControlKey) { riq_control_right.Show(); }
            if (e.KeyCode == Keys.D) { riq_d.Show(); }
            if (e.KeyCode == Keys.Delete) { riq_delete.Show(); }
            if (e.KeyCode == Keys.OemPeriod) { riq_dot.Show(); }
            if (e.KeyCode == Keys.Down) { riq_down.Show(); }
            if (e.KeyCode == Keys.E) { riq_e.Show(); }
            if (e.KeyCode == Keys.End) { riq_end.Show(); }
            if (e.KeyCode == Keys.Enter) { riq_enter.Show(); }
            if (e.KeyCode == Keys.Oemplus) { riq_equal.Show(); }
            if (e.KeyCode == Keys.Escape) { riq_escape.Show(); }
            if (e.KeyCode == Keys.F) { riq_f.Show(); }
            if (e.KeyCode == Keys.F1) { riq_f1.Show(); }
            if (e.KeyCode == Keys.F2) { riq_f2.Show(); }
            if (e.KeyCode == Keys.F3) { riq_f3.Show(); }
            if (e.KeyCode == Keys.F4) { riq_f4.Show(); }
            if (e.KeyCode == Keys.F5) { riq_f5.Show(); }
            if (e.KeyCode == Keys.F6) { riq_f6.Show(); }
            if (e.KeyCode == Keys.F7) { riq_f7.Show(); }
            if (e.KeyCode == Keys.F8) { riq_f8.Show(); }
            if (e.KeyCode == Keys.F9) { riq_f9.Show(); }
            if (e.KeyCode == Keys.F10) { riq_f10.Show(); }
            if (e.KeyCode == Keys.F11) { riq_f11.Show(); }
            if (e.KeyCode == Keys.F12) { riq_f12.Show(); }
            if (e.KeyCode == Keys.G) { riq_g.Show(); }
            if (e.KeyCode == Keys.H) { riq_h.Show(); }
            if (e.KeyCode == Keys.Home) { riq_home.Show(); }
            if (e.KeyCode == Keys.I) { riq_i.Show(); }
            if (e.KeyCode == Keys.Insert) { riq_insert.Show(); }
            if (e.KeyCode == Keys.J) { riq_j.Show(); }
            if (e.KeyCode == Keys.K) { riq_k.Show(); }
            if (e.KeyCode == Keys.L) { riq_l.Show(); }
            if (e.KeyCode == Keys.Left) { riq_left.Show(); }
            if (e.KeyCode == Keys.Menu) { riq_list.Show(); }
            if (e.KeyCode == Keys.M) { riq_m.Show(); }
            if (e.KeyCode == Keys.OemMinus) { riq_minus.Show(); }
            if (e.KeyCode == Keys.N) { riq_n.Show(); }
            if (e.KeyCode == Keys.NumLock) { riq_num_lock.Show(); }
            if (e.KeyCode == Keys.NumPad0) { riq_num_lock_0.Show(); }
            if (e.KeyCode == Keys.NumPad1) { riq_num_lock_1.Show(); }
            if (e.KeyCode == Keys.NumPad2) { riq_num_lock_2.Show(); }
            if (e.KeyCode == Keys.NumPad3) { riq_num_lock_3.Show(); }
            if (e.KeyCode == Keys.NumPad4) { riq_num_lock_4.Show(); }
            if (e.KeyCode == Keys.NumPad5) { riq_num_lock_5.Show(); }
            if (e.KeyCode == Keys.NumPad6) { riq_num_lock_6.Show(); }
            if (e.KeyCode == Keys.NumPad7) { riq_num_lock_7.Show(); }
            if (e.KeyCode == Keys.NumPad8) { riq_num_lock_8.Show(); }
            if (e.KeyCode == Keys.NumPad9) { riq_num_lock_9.Show(); }
            if (e.KeyCode == Keys.Multiply) { riq_num_lock_asterisk.Show(); }
            if (e.KeyCode == Keys.Decimal) { riq_num_lock_dot.Show(); }
            if (e.KeyCode == Keys.Return) { riq_num_lock_enter.Show(); }
            if (e.KeyCode == Keys.Subtract) { riq_num_lock_minus.Show(); }
            if (e.KeyCode == Keys.Add) { riq_num_lock_plus.Show(); }
            if (e.KeyCode == Keys.Divide) { riq_num_lock_slash.Show(); }
            if (e.KeyCode == Keys.O) { riq_o.Show(); }
            if (e.KeyCode == Keys.Oemtilde) { riq_oemtilde.Show(); }
            if (e.KeyCode == Keys.P) { riq_p.Show(); }
            if (e.KeyCode == Keys.PageDown) { riq_page_down.Show(); }
            if (e.KeyCode == Keys.PageUp) { riq_page_up.Show(); }
            if (e.KeyCode == Keys.Pause) { riq_pause.Show(); }
            if (e.KeyCode == Keys.PrintScreen) { riq_print_screen.Show(); }
            if (e.KeyCode == Keys.Q) { riq_q.Show(); }
            if (e.KeyCode == Keys.R) { riq_r.Show(); }
            if (e.KeyCode == Keys.Right) { riq_right.Show(); }
            if (e.KeyCode == Keys.S) { riq_s.Show(); }
            if (e.KeyCode == Keys.Scroll) { riq_scroll_lock.Show(); }
            if (e.KeyCode == Keys.OemSemicolon) { riq_semicolon.Show(); }
            if (e.KeyCode == Keys.LShiftKey) { riq_shift_left.Show(); }
            if (e.KeyCode == Keys.RShiftKey) { riq_shift_right.Show(); }
            if (e.KeyCode == Keys.OemQuestion) { riq_slash.Show(); }
            if (e.KeyCode == Keys.Space) { riq_space.Show(); }
            if (e.KeyCode == Keys.T) { riq_t.Show(); }
            if (e.KeyCode == Keys.Tab) { riq_tab.Show(); }
            if (e.KeyCode == Keys.U) { riq_u.Show(); }
            if (e.KeyCode == Keys.Up) { riq_up.Show(); }
            if (e.KeyCode == Keys.V) { riq_v.Show(); }
            if (e.KeyCode == Keys.W) { riq_w.Show(); }
            if (e.KeyCode == Keys.LWin) { riq_windows_left.Show(); }
            if (e.KeyCode == Keys.RWin) { riq_windows_right.Show(); }
            if (e.KeyCode == Keys.X) { riq_x.Show(); }
            if (e.KeyCode == Keys.Y) { riq_y.Show(); }
            if (e.KeyCode == Keys.Z) { riq_z.Show(); }
        }
        private void FormUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == riqMoveKey) { riqMove = false; riq_layout.Hide(); }
        }
        private void FormDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == riqMoveKey) { riqMove = true; riq_layout.Show(); }
            if (e.KeyCode == Keys.PageDown && riqMove == true) { SendToBack(); }
            if (e.KeyCode == Keys.PageUp && riqMove == true) { BringToFront(); this.TopMost = true; }
            if (e.KeyCode == Keys.Escape && riqMove == true) { riqNotifyIcon.Visible = false; this.Close(); }
            if (e.KeyCode == Keys.A && riqMove == true)
            {
                if (winLocX <= riqStep)
                {
                    winLocX = 0;
                    this.Location = new Point(winLocX, winLocY);
                }
                else
                {
                    winLocX -= riqStep;
                    this.Location = new Point(winLocX, winLocY);
                }
            }
            if (e.KeyCode == Keys.D && riqMove == true)
            {
                if (winLocX + riqW >= winW - riqStep)
                {
                    winLocX = winW - riqW;
                    this.Location = new Point(winLocX, winLocY);
                }
                else
                {
                    winLocX += riqStep;
                    this.Location = new Point(winLocX, winLocY);
                }
            }
            if (e.KeyCode == Keys.W && riqMove == true)
            {
                if (winLocY <= riqStep)
                {
                    winLocY = 0;
                    this.Location = new Point(winLocX, winLocY);
                }
                else
                {
                    winLocY -= riqStep;
                    this.Location = new Point(winLocX, winLocY);
                }
            }
            if (e.KeyCode == Keys.S && riqMove == true)
            {
                if (winLocY + riqH >= winH - riqStep)
                {
                    winLocY = winH - riqH;
                    this.Location = new Point(winLocX, winLocY);
                }
                else
                {
                    winLocY += riqStep;
                    this.Location = new Point(winLocX, winLocY);
                }
            }
        }
        private void RiqClick(object sender, EventArgs e)
        {
            riqNotifyIcon.Visible = false;  this.Close();
        }
        private void Help(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://riqboard.com/");
        }
        private void Donate(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://riqboard.com/");
        }
        private void HideMe(object sender, EventArgs e)
        {
            if (this.TopMost == false) { this.SendToBack(); }
            else { this.TopMost = false; this.SendToBack(); }
        }
        private void ShowMe(object sender, EventArgs e)
        {
            this.BringToFront();
        }
        private void TopMe(object sender, EventArgs e)
        {
            this.TopMost = true;
        }
        #region dllimport
        [DllImport("gdi32.dll")]
        static extern IntPtr AddFontMemResourceEx(
            IntPtr pbFont,
            uint cbFont,
            IntPtr pdv,
            [In] ref uint pcFonts);
        #endregion
    }
}