﻿using System;
using Intersect.GameObjects;
using IntersectClientExtras.File_Management;
using IntersectClientExtras.GenericClasses;
using IntersectClientExtras.Graphics;
using IntersectClientExtras.Gwen.Control;
using IntersectClientExtras.Gwen.Control.EventArguments;
using IntersectClientExtras.Input;
using Intersect_Client.Classes.General;
using Intersect_Client.Classes.Networking;
using Intersect_Client.Classes.UI.Game;

namespace Intersect.Client.Classes.UI.Game.Character
{
    public class EquipmentItem
    {
        private WindowControl mCharacterWindow;
        private int mCurrentItem = -1;
        private ItemDescWindow mDescWindow;
        private int[] mStatBoost = new int[Options.MaxStats];
        private bool mTexLoaded;
        public ImagePanel ContentPanel;
        private int mYindex;
        public ImagePanel Pnl;

        public EquipmentItem(int index, WindowControl characterWindow)
        {
            mYindex = index;
            mCharacterWindow = characterWindow;
        }

        public void Setup()
        {
            Pnl.HoverEnter += pnl_HoverEnter;
            Pnl.HoverLeave += pnl_HoverLeave;
            Pnl.RightClicked += pnl_RightClicked;

            ContentPanel = new ImagePanel(Pnl, "EquipmentIcon");
            Pnl.SetToolTipText(Options.EquipmentSlots[mYindex]);
        }

        void pnl_RightClicked(Base sender, ClickedEventArgs arguments)
        {
            PacketSender.SendUnequipItem(mYindex);
        }

        void pnl_HoverLeave(Base sender, EventArgs arguments)
        {
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
        }

        void pnl_HoverEnter(Base sender, EventArgs arguments)
        {
            if (Globals.InputManager.MouseButtonDown(GameInput.MouseButtons.Left))
            {
                return;
            }
            if (mDescWindow != null)
            {
                mDescWindow.Dispose();
                mDescWindow = null;
            }
            if (ItemBase.Lookup.Get<ItemBase>(mCurrentItem) == null) return;
            mDescWindow = new ItemDescWindow(mCurrentItem, 1, mCharacterWindow.X - 255, mCharacterWindow.Y, mStatBoost, ItemBase.GetName(mCurrentItem));
        }

        public FloatRect RenderBounds()
        {
            FloatRect rect = new FloatRect()
            {
                X = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).X,
                Y = Pnl.LocalPosToCanvas(new IntersectClientExtras.GenericClasses.Point(0, 0)).Y,
                Width = Pnl.Width,
                Height = Pnl.Height
            };
            return rect;
        }

        public void Update(int currentItem, int[] statBoost)
        {
            if (currentItem != mCurrentItem || !mTexLoaded)
            {
                mCurrentItem = currentItem;
                mStatBoost = statBoost;
                var item = ItemBase.Lookup.Get<ItemBase>(mCurrentItem);
                if (item != null)
                {
                    GameTexture itemTex = Globals.ContentManager.GetTexture(GameContentManager.TextureType.Item,
                        item.Pic);
                    if (itemTex != null)
                    {
                        ContentPanel.Show();
                        ContentPanel.Texture = itemTex;
                    }
                    else
                    {
                        ContentPanel.Hide();
                    }
                }
                else
                {
                    ContentPanel.Hide();
                }
                mTexLoaded = true;
            }
        }
    }
}