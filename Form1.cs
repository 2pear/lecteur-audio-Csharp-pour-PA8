using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Microsoft.DirectX.AudioVideoPlayback;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        private Microsoft.DirectX.AudioVideoPlayback.Audio monfichier;
        private SortedDictionary <int, string> playlist;
        private int index;
        public Form1()
        {
            InitializeComponent();
            playlist = new SortedDictionary<int, string>();
            index = 0;
        }

        //Lance la lecture
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if(playlist.Count>0)
                lancer_fichier();
        }
        //Arret de la lecture, pour reprise au meme endroit
        private void buttonPause_Click(object sender, EventArgs e)
        {
            pause_fichier();
        }
        //Arret de la lecture,index reste sur le meme fichier
        private void buttonStop_Click(object sender, EventArgs e)
        {
            arreter_fichier();
        }
        //Titre precedent
        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (playlist.ContainsKey(index - 1))
            {
                precedent();
            }
        }
        //Titre suivant
        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (playlist.ContainsKey(index + 1))
            {
                suivant();
            }

        }
        private void buttonSuppr_Click(object sender, EventArgs e)
        {
            try
            {

                int indice = Aff_playlist.SelectedIndices[0];
                if (playlist.ContainsKey(indice))
                {
                    for (int i = indice + 1; i < playlist.Count; i++)
                    {
                        playlist[i - 1] = playlist[i];
                    }
                    playlist.Remove(playlist.Count - 1);
                    lister();
                }
            }
            catch
            {
                MessageBox.Show("Aucun fichier selectionné", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void buttonUp_Click(object sender, EventArgs e)
        {
            try
            {
                int indice = Aff_playlist.SelectedIndices[0];
                if (indice != index && indice != 0)
                {
                    string s = playlist[indice];
                    playlist[indice] = playlist[indice - 1];
                    playlist[indice - 1] = s;
                    if (indice == index + 1)
                        index++;
                    lister();
                    Aff_playlist.Items[indice-1].Selected = true;
                }
                else
                {
                    MessageBox.Show("Impossible de monter ce fichier", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Aucun fichier n'est selectionné", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
           
        }
        private void buttonDown_Click(object sender, EventArgs e)
        {
            try
            {
                int indice = Aff_playlist.SelectedIndices[0];
                if (indice != index && indice != playlist.Count-1)
                {
                    string s = playlist[indice];
                    playlist[indice] = playlist[indice+1];
                    playlist[indice+1] = s;
                    if (indice == index - 1)
                        index--;
                    lister();
                    Aff_playlist.Items[indice + 1].Selected = true;
                }
                else
                {
                    MessageBox.Show("Impossible de descendre ce fichier ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Aucun fichier n'est selectionné", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }



        //permet d'avancer manuellement
        private void SlideBar_Scroll(object sender, EventArgs e)
        {
            if (monfichier != null)
                monfichier.CurrentPosition = (int)Barre_avancement.Value;
        }
        //Affichage du temps
        private void timer1_Tick(object sender, EventArgs e)
        {
                if (monfichier != null)
                {
                    int i = (int)monfichier.CurrentPosition;
                    int min = i / 60;
                    int sec = i % 60;
                    Barre_avancement.Value = i;
                    if (min < 10 && sec < 10)
                    {
                        temps.Text = "0" + min.ToString() + ":0" + sec.ToString();
                    }
                    if (min < 10 && sec > 10)
                    {
                        temps.Text = "0" + min.ToString() + ":" + sec.ToString();
                    }
                    if (sec < 10 && min > 10)
                    {
                        temps.Text = min.ToString() + ":0" + sec.ToString();
                    }
                    if (min > 10 && sec > 10)
                    {
                        temps.Text = min.ToString() + ":" + sec.ToString();
                    }
                
                    if (monfichier.CurrentPosition >= monfichier.Duration)
                    {
                        monfichier.Stop();
                        if (playlist.ContainsKey(index+1))
                        {
                            index++;
                            try
                            {
                                monfichier = new Audio(playlist[index]);
                            }
                            catch
                            {
                                MessageBox.Show("Fichier invalide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            
                            RAZslidebar();
                            lister();
                            monfichier.Play();
                        }

                    }
                }
        }
        //Remplissage de la playliste
       
        /*Charge le fichier,lance la lecture et lance le timer*/
        private void lancer_fichier()
        {
            if (monfichier == null)
            {
                if (playlist.ContainsKey(index))
                {
                    try
                    {
                        monfichier = new Audio(playlist[index]);
                        monfichier.Play();
                    }
                    catch
                    {
                        MessageBox.Show("Fichier invalide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
            }
            else
            {
                if (monfichier == null)
                    textBox3.Text = "il est nul";
                monfichier.Play();

            }
            timer1.Start();
        }
        private void arreter_fichier()
        {
            if (monfichier != null)
            {
                monfichier.Stop();
                timer1.Stop();
            }
            Barre_avancement.Value = 0;
            temps.Text = "00:00";
        }
        private void pause_fichier()
        {
            if (monfichier != null)
            {
                monfichier.Pause();
                timer1.Stop();
            }
        }
        private void lister()
        {
            Aff_playlist.Items.Clear();

            foreach (KeyValuePair<int, string> k in playlist)
            {

                ListViewItem itm = new ListViewItem();
                if (k.Key == index)
                {
                    itm.SubItems[0].BackColor = Color.LightGreen;
                }
                itm.SubItems[0].Text = k.Value;
                Aff_playlist.Items.Add(itm);
            }
        }
        
        //Préparation Slidebar pour nouveau fichier
        private void RAZslidebar()
        {
                Barre_avancement.Maximum = (int)monfichier.Duration;
                Barre_avancement.Value = 0;
        }
       
        //Ajout d'un fichier par glisser déposer
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            textBox3.Text=e.Data.GetType().ToString();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string s in files)
                {
                    if (s.Contains(".mp3") || s.Contains(".wma") || s.Contains(".wav"))
                    {
                        playlist.Add(playlist.Count, s);
                        
                    }
                }
                lister();
                textBox3.Text = files[0];
            }
        }
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if( e.Data.GetDataPresent(DataFormats.FileDrop) ) 
            {
                  e.Effect = DragDropEffects.Copy;
            }
        }
        //Lancement de fichier par double clik
        private void listView1_DoubleClick(object sender, EventArgs e)
                {
                    if(playlist.ContainsKey(Aff_playlist.SelectedIndices[0]))
                    {
                        if(monfichier!=null)
                        {
                            try
                            {
                                monfichier.Stop();
                                monfichier.Dispose();
                                monfichier = null;
                            }
                            catch 
                            {
         
                            }
                        }
                        
                        index = Aff_playlist.SelectedIndices[0];
                        lister();
                        lancer_fichier();
                    }
                        
                }
        
        //menu
        private void quitterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog exploreur = new OpenFileDialog();
            exploreur.Multiselect = true;
            exploreur.Filter = ("Fichier Audio|*.mp3;*.wma;*.wav");
            exploreur.RestoreDirectory = true;
            if (exploreur.ShowDialog() == DialogResult.OK)
            {
                List<string> l = new List<string>();
                l.AddRange(exploreur.FileNames);
                int i = playlist.Count;
                foreach (string s in l)
                {
                    playlist.Add(i, s);
                    i++;
                }
                if (monfichier == null)
                {
                    try
                    {
                        monfichier = new Audio(playlist[0]);
                    }
                    catch 
                    {
                        MessageBox.Show("Le premier fichier selectionné est invalide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (playlist.Count != 1)
                        {
                            playlist[0] = playlist[playlist.Count - 1];
                        }
                        playlist.Remove(playlist.Count-1);
                        lister();
                    }
                }
            }
            lister();
            if (monfichier != null)
                Barre_avancement.Maximum = (int)monfichier.Duration;
        }
        private void aProposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Lecteur de MP3 codé en c#\nBy Julien", "A propos", MessageBoxButtons.OK);
        }

        
        //Passage a la liste suivante ou precedent,ainsi que le traitement d'un fichier invalide
        private void precedent()
        {
            if (playlist.Count != 0 && playlist.ContainsKey(index - 1))
            {
                try
                {
                    monfichier.Stop();
                    timer1.Stop();
                    monfichier.Dispose();
                    index--;
                    monfichier = new Audio(playlist[index]);

                }
                catch
                {
                    MessageBox.Show("Fichier invalide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    supprimer_fichier();
                }
                RAZslidebar();
                lister();
                monfichier.Play();
                timer1.Start();
            }

        }
        private void suivant()
        {
            if (playlist.Count != 0 && playlist.ContainsKey(index + 1))
            {
                try
                {
                    monfichier.Stop();
                    timer1.Stop();
                    monfichier.Dispose();
                    index++;
                    monfichier = new Audio(playlist[index]);

                }
                catch
                {
                    MessageBox.Show("Fichier invalide", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    supprimer_fichier();
                }
                RAZslidebar();
                lister();
                monfichier.Play();
                timer1.Start();
            }

        }
       
        //suppression par index
        private void supprimer(int i_sup)
        {
            try
            {

                int indice = Aff_playlist.SelectedIndices[0];
                if (indice!= i_sup)
                {
                    if (playlist.ContainsKey(indice))
                    {
                        for (int i = indice + 1; i < playlist.Count; i++)
                        {
                            playlist[i - 1] = playlist[i];
                        }
                        playlist.Remove(playlist.Count-1);
                        lister();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Aucun fichier selectionné", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        //suppression pour fichier illisible
        private void supprimer_fichier()
        {
            int last=playlist.Count-1;
            if (index == last)
            {
                playlist.Remove(last);
                if (index != 0)
                { 
                    index--;
                }
                
            }
            else
            {
                for (int i = index; i < playlist.Count - 1; i++)
                {
                    playlist[i] = playlist[i + 1];
                }
                playlist.Remove(last);
            }
            
        }

    }
}
      