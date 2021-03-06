﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Visualization
{
	/// <summary>
	/// ベクトル図を描画するためのクラスです。
	/// </summary>
	class VectorField : Field
	{
		/// <summary>
		/// ベクトル図の設定を行う VectorSetting クラスのインスタンスを取得します。
		/// </summary>
		public VectorSetting Setting { get; private set; }

		/// <summary>
		/// 新しい VectorField クラスのインスタンスを作成します。
		/// </summary>
		public VectorField()
		{
			this.Setting = new VectorSetting();
		}

		/// <summary>
		/// ビットマップにベクトル図を描画します。
		/// </summary>
		/// <param name="bmp">描画先の System.Drawing.Bitmap</param>
		/// <param name="data">元となる 2 次元配列データ</param>
		/// <exception cref="Visualization.VectorDataNotFoundException">ベクトルデータがないときに投げられます。</exception>
		public override void DrawTo( Canvas canvas, IDataSheet data )
		{
			Bitmap bmp = canvas.Bitmap;
			if( !this.Setting.Show )
			{
				return;
			}
			if( !data.HasVectorData )
			{
				throw new VectorDataNotFoundException( "ベクトルデータがありません" );
			}
			if( !this.Setting.IsLengthFixed )
			{
				this.Setting.ReferredLength = data.MaxVector;
			}
			Graphics g = Graphics.FromImage( bmp );
			Pen p = new Pen( Setting.LineColor );
			Brush b = new SolidBrush( Setting.InnerColor );

			ArrowDrawer d = new ArrowDrawer( g, this.Setting, data.MaxVector, data.MinVector );

			int start = (this.Setting.Interval > 1) ? this.Setting.Offset : 0;
			int step = this.Setting.Interval;

			HashSet<int> iSet = new HashSet<int>();
			HashSet<int> jSet = new HashSet<int>();

			if( this.Setting.FixedBound )
			{
				iSet.Add( 0 );
				jSet.Add( 0 );
			}
			for( int i=start; i<data.Columns; i += step )
			{
				iSet.Add( i );
			}
			for( int j=start; j<data.Rows; j += step )
			{
				jSet.Add( j );
			}
			if( this.Setting.FixedBound )
			{
				iSet.Add( data.Columns-1 );
				jSet.Add( data.Rows-1 );
			}

			foreach( var i in iSet )
			{
				foreach( var j in jSet )
				{
					Point pt = canvas.AsBitmapCoord( data.GetX( i ), data.GetY( j ) );
					double u = data.GetU( i, j );
					double v = data.GetV( i, j );
					double len = Math.Sqrt( u*u + v*v );
					d.DrawArrow( pt, u, v );
				}
			}
		}
	}
}
