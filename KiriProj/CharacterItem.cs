﻿/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using KiriFig.Model;

namespace KiriProj
{
    public delegate void FigureItemModifiedHandler( object sender, FigureItemModifiedEventArgs e );

    public class FigureItemModifiedEventArgs : EventArgs
    {
        public FigureItem FigureItem { get; }

        public FigureItemModifiedEventArgs( FigureItem figureItem )
        {
            FigureItem = figureItem;
        }
    }

    public enum CompletionState
    {
        Unknown,
        Incomplete,
        Complete
    }

    public class CharacterItem
    {
        private const string CharacterFolderSearchPattern = "U_*";
        private const string GenericNameSearchPattern = "piece*.kefig";
        private const string MasterFileName = "master.kefigm";
        private const string FigureSearchPattern = "*.kefig";

        private List<FigureItem> _figureItems = new List<FigureItem>();
        private bool _deleted;

        public event FigureItemModifiedHandler FigureItemModified;
        public event EventHandler Deleted;

        public Project Project { get; }
        public uint CodePoint { get; }
        public string RootPath { get; }

        public FigureItemBase MasterFigureItem { get; }
        public IReadOnlyList<FigureItem> PieceFigureItems => _figureItems;

        public CompletionState Completion { get; set; }

        private CharacterItem( Project project, uint codePoint, bool enumPieces = false )
        {
            string charRoot = GetRootPath( project, codePoint );
            string masterPath = Path.Combine( charRoot, MasterFileName );

            if ( enumPieces )
                FillPieces( charRoot );

            Project = project;
            CodePoint = codePoint;
            RootPath = charRoot;

            MasterFigureItem = new FigureItemBase( masterPath, this );
        }

        private void FillPieces( string rootPath )
        {
            var charRootInfo = new DirectoryInfo( rootPath );

            foreach ( var fileInfo in charRootInfo.EnumerateFiles( FigureSearchPattern ) )
            {
                _figureItems.Add( new FigureItem( fileInfo.FullName, this ) );
            }
        }

        internal static IEnumerable<CharacterItem> EnumerateCharacterItems( Project project )
        {
            var charsFolderInfo = new DirectoryInfo( project.CharactersFolderPath );

            foreach ( var dirInfo in charsFolderInfo.EnumerateDirectories( CharacterFolderSearchPattern ) )
            {
                string substring = dirInfo.Name.Substring( 2 );
                uint number;

                if ( !uint.TryParse( substring, NumberStyles.HexNumber, null, out number ) )
                    continue;

                var item = new CharacterItem( project, number, true );

                yield return item;
            }

            yield break;
        }

        private static string MakeCharacterFileName( uint codePoint )
        {
            return string.Format( "U_{0:X6}", codePoint );
        }

        private static string GetRootPath( Project project, uint codePoint )
        {
            string name = MakeCharacterFileName( codePoint );
            string charPath = Path.Combine( project.CharactersFolderPath, name );
            return charPath;
        }

        internal static CharacterItem Make( Project project, uint codePoint )
        {
            string rootPath = GetRootPath( project, codePoint );

            if ( Directory.Exists( rootPath ) )
                throw new ApplicationException();

            string figurePath = Path.Combine( rootPath, MasterFileName );

            Figure figure = FigureUtils.MakeMasterFigure(
                project.FontPath,
                project.FaceIndex,
                codePoint );

            var document = new FigureDocument();

            document.Figure = figure;

            var characterItem = new CharacterItem( project, codePoint );
            var figureItem = new FigureItem( figurePath, characterItem );

            try
            {
                Directory.CreateDirectory( rootPath );

                figureItem.Save( document );
            }
            catch ( Exception )
            {
                Directory.Delete( rootPath, true );
            }

            return characterItem;
        }

        private static void DeleteTree( Project project, uint codePoint )
        {
            string rootPath = GetRootPath( project, codePoint );

            Directory.Delete( rootPath, true );
        }

        public static string FindNextFileName( Project project, uint codePoint )
        {
            string rootPath = GetRootPath( project, codePoint );

            DirectoryInfo rootInfo = new DirectoryInfo( rootPath );
            var numbers = new List<int>();

            foreach ( var fileInfo in rootInfo.EnumerateFiles( GenericNameSearchPattern ) )
            {
                string fileName = fileInfo.Name;
                int lastDot = fileName.LastIndexOf( '.' );
                string substr = fileName.Substring( 5, lastDot - 5 );

                if ( int.TryParse( substr, NumberStyles.None, null, out int n ) && n != 0 )
                    numbers.Add( n );
            }

            int number = 1;

            if ( numbers.Count > 0 )
            {
                numbers.Sort();

                for ( int i = 0; i < numbers.Count; i++ )
                {
                    if ( number != numbers[i] )
                        break;

                    number++;
                }
            }

            return string.Format( "piece{0}.kefig", number );
        }

        public FigureItem AddItem( string name, FigureItemBase template )
        {
            if ( string.IsNullOrEmpty( name ) )
                throw new ArgumentException();

            // Fail if the item exists.

            foreach ( var item in _figureItems )
            {
                if ( name.Equals( item.Name, StringComparison.OrdinalIgnoreCase ) )
                    throw new ApplicationException();
            }

            var figureItem = FigureItem.Make( this, name, template );

            _figureItems.Add( figureItem );
            Project.NotifyItemModified( this );

            return figureItem;
        }

        public void DeleteItem( string name )
        {
            for ( int i = 0; i < PieceFigureItems.Count; i++ )
            {
                var item = PieceFigureItems[i];

                if ( name.Equals( item.Name, StringComparison.OrdinalIgnoreCase ) )
                {
                    DeleteItem( item, i );
                    return;
                }
            }

            throw new ApplicationException();
        }

        public void DeleteItem( FigureItem item )
        {
            int index = _figureItems.IndexOf( item );
            DeleteItem( item, index );
        }

        public void DeleteItem( FigureItem item, int index )
        {
            if ( Completion == CompletionState.Complete )
                Completion = CompletionState.Unknown;

            _figureItems.RemoveAt( index );
            item.Delete();

            if ( !_deleted )
                Project.NotifyItemModified( this );
        }

        internal void Delete()
        {
            if ( _deleted )
                return;

            _deleted = true;

            var items = _figureItems.ToArray();

            foreach ( var figureItem in items )
            {
                DeleteItem( figureItem );
            }

            CharacterItem.DeleteTree( Project, CodePoint );

            Deleted?.Invoke( this, EventArgs.Empty );
        }

        internal void NotifyItemModified( FigureItem figureItem )
        {
            Completion = CompletionState.Unknown;

            if ( FigureItemModified != null )
            {
                var args = new FigureItemModifiedEventArgs( figureItem );

                FigureItemModified?.Invoke( this, args );
            }
        }
    }
}
