//--------------------------------------------------------------------------------------------------
/**
@file	q3Body.cpp

@author	Randy Gaul
@date	10/10/2014

	Copyright (c) 2014 Randy Gaul http://www.randygaul.net

	This software is provided 'as-is', without any express or implied
	warranty. In no event will the authors be held liable for any damages
	arising from the use of this software.

	Permission is granted to anyone to use this software for any purpose,
	including commercial applications, and to alter it and redistribute it
	freely, subject to the following restrictions:
	  1. The origin of this software must not be misrepresented; you must not
	     claim that you wrote the original software. If you use this software
	     in a product, an acknowledgment in the product documentation would be
	     appreciated but is not required.
	  2. Altered source versions must be plainly marked as such, and must not
	     be misrepresented as being the original software.
	  3. This notice may not be removed or altered from any source distribution.
*/
//--------------------------------------------------------------------------------------------------

#include "q3Body.h"
#include "../scene/q3Scene.h"
#include "q3Contact.h"
#include "../broadphase/q3BroadPhase.h"
#include "../collision/q3Box.h"

//--------------------------------------------------------------------------------------------------
// q3Body
//--------------------------------------------------------------------------------------------------
q3Body::q3Body( const q3BodyDef& def, q3Scene* scene )
{
	m_q.Set( q3Normalize( def.axis ), def.angle );
	m_tx.rotation = m_q.ToMat3( );
	m_tx.position = def.position;
	m_sleepTime = r32( 0.0 );
	m_layers = def.layers;
	m_userData = def.userData;
	m_scene = scene;
	m_flags = 0;

	if ( def.bodyType == eDynamicBody )
		m_flags |= q3Body::eDynamic;

	else
	{
		if ( def.bodyType == eStaticBody )
		{
			m_flags |= q3Body::eStatic;
		}

		else if ( def.bodyType == eKinematicBody )
			m_flags |= q3Body::eKinematic;
	}

	if ( def.allowSleep )
		m_flags |= eAllowSleep;

	if ( def.awake )
		m_flags |= eAwake;

	if ( def.active )
		m_flags |= eActive;

	if ( def.lockAxisX )
		m_flags |= eLockAxisX;

	if ( def.lockAxisY )
		m_flags |= eLockAxisY;

	if ( def.lockAxisZ )
		m_flags |= eLockAxisZ;

	m_boxes = NULL;
	m_contactList = NULL;
	q3Identity(m_localCenter);
	m_worldCenter = def.position;
}

//--------------------------------------------------------------------------------------------------
const q3Box* q3Body::AddBox( const q3BoxDef& def )
{
	q3AABB aabb;
	q3Box* box = (q3Box*)m_scene->m_heap.Allocate( sizeof( q3Box ) );
	box->local = def.m_tx;
	box->e = def.m_e;
	box->next = m_boxes;
	m_boxes = box;
	box->ComputeAABB( m_tx, &aabb );

	box->body = this;

	m_scene->m_contactManager.m_broadphase.InsertBox( box, aabb );
	m_scene->m_newBox = true;

	return box;
}

//--------------------------------------------------------------------------------------------------
void q3Body::RemoveBox( const q3Box* box )
{
	assert( box );
	assert( box->body == this );

	q3Box* node = m_boxes;

	bool found = false;
	if ( node == box )
	{
		m_boxes = node->next;
		found = true;
	}

	else
	{
		while ( node )
		{
			if ( node->next == box )
			{
				node->next = box->next;
				found = true;
				break;
			}

			node = node->next;
		}
	}

	// This shape was not connected to this body.
	assert( found );

	// Remove all contacts associated with this shape
	q3ContactEdge* edge = m_contactList;
	while ( edge )
	{
		q3ContactConstraint* contact = edge->constraint;
		edge = edge->next;

		q3Box* A = contact->A;
		q3Box* B = contact->B;

		if ( box == A || box == B )
			m_scene->m_contactManager.RemoveContact( contact );
	}

	m_scene->m_contactManager.m_broadphase.RemoveBox( box );

	m_scene->m_heap.Free( (void*)box );
}

//--------------------------------------------------------------------------------------------------
void q3Body::RemoveAllBoxes( )
{
	while ( m_boxes )
	{
		q3Box* next = m_boxes->next;

		m_scene->m_contactManager.m_broadphase.RemoveBox( m_boxes );
		m_scene->m_heap.Free( (void*)m_boxes );

		m_boxes = next;
	}

	m_scene->m_contactManager.RemoveContactsFromBody( this );
}

//--------------------------------------------------------------------------------------------------
void q3Body::SetToAwake( )
{
	if( !(m_flags & eAwake) )
	{
		m_flags |= eAwake;
		m_sleepTime = r32( 0.0 );
	}
}

//--------------------------------------------------------------------------------------------------
void q3Body::SetToSleep( )
{
	m_flags &= ~eAwake;
	m_sleepTime = r32( 0.0 );
}

//--------------------------------------------------------------------------------------------------
bool q3Body::IsAwake( ) const
{
	return m_flags & eAwake ? true : false;
}

//--------------------------------------------------------------------------------------------------
const q3Vec3 q3Body::GetLocalPoint( const q3Vec3& p ) const
{
	return q3MulT( m_tx, p );
}

//--------------------------------------------------------------------------------------------------
const q3Vec3 q3Body::GetLocalVector( const q3Vec3& v ) const
{
	return q3MulT( m_tx.rotation, v );
}

//--------------------------------------------------------------------------------------------------
const q3Vec3 q3Body::GetWorldPoint( const q3Vec3& p ) const
{
	return q3Mul( m_tx, p );
}

//--------------------------------------------------------------------------------------------------
const q3Vec3 q3Body::GetWorldVector( const q3Vec3& v ) const
{
	return q3Mul( m_tx.rotation, v );
}

//--------------------------------------------------------------------------------------------------
bool q3Body::CanCollide( const q3Body *other ) const
{
	if ( this == other )
		return false;

	// Every collision must have at least one dynamic body involved
	if ( !(m_flags & eDynamic) && !(other->m_flags & eDynamic) )
		return false;

	if ( !(m_layers & other->m_layers) )
		return false;

	return true;
}

//--------------------------------------------------------------------------------------------------
const q3Transform q3Body::GetTransform( ) const
{
	return m_tx;
}

//--------------------------------------------------------------------------------------------------
void q3Body::SetPosition( const q3Vec3& position )
{
	m_worldCenter = position;

	SynchronizeProxies( );
}

void q3Body::SetRotation(const q3Quaternion& q)
{
	m_q = q;
	m_tx.rotation = m_q.ToMat3();

	SynchronizeProxies();
}

//--------------------------------------------------------------------------------------------------
void q3Body::SetTransform( const q3Vec3& position, const q3Quaternion& q)
{
	m_worldCenter = position;
	m_q = q;
	m_tx.rotation = m_q.ToMat3();

	SynchronizeProxies( );
}

//--------------------------------------------------------------------------------------------------
i32 q3Body::GetFlags( ) const
{
	return m_flags;
}

//--------------------------------------------------------------------------------------------------
void q3Body::SetLayers( i32 layers )
{
	m_layers = layers;
}

//--------------------------------------------------------------------------------------------------
i32 q3Body::GetLayers( ) const
{
	return m_layers;
}

//--------------------------------------------------------------------------------------------------
const q3Quaternion q3Body::GetQuaternion( ) const
{
	return m_q;
}

//--------------------------------------------------------------------------------------------------
void* q3Body::GetUserData( ) const
{
  return m_userData;
}

//--------------------------------------------------------------------------------------------------
void q3Body::Render( q3Render* render ) const
{
	bool awake = IsAwake( );
	q3Box* box = m_boxes;

	while ( box )
	{
		box->Render( m_tx, awake, render );
		box = box->next;
	}
}

//--------------------------------------------------------------------------------------------------
void q3Body::Dump( FILE* file, i32 index ) const
{
	fprintf( file, "{\n" );
	fprintf( file, "\tq3BodyDef bd;\n" );

	switch ( m_flags & (eStatic | eDynamic | eKinematic) )
	{
	case eStatic:
		fprintf( file, "\tbd.bodyType = q3BodyType( %d );\n", eStaticBody );
		break;

	case eDynamic:
		fprintf( file, "\tbd.bodyType = q3BodyType( %d );\n", eDynamicBody );
		break;

	case eKinematic:
		fprintf( file, "\tbd.bodyType = q3BodyType( %d );\n", eKinematicBody );
		break;
	}

	fprintf( file, "\tbd.position.Set( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", m_tx.position.x, m_tx.position.y, m_tx.position.z );
	q3Vec3 axis;
	r32 angle;
	m_q.ToAxisAngle( &axis, &angle );
	fprintf( file, "\tbd.axis.Set( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", axis.x, axis.y, axis.z );
	fprintf( file, "\tbd.angle = r32( %.15lf );\n", angle );
	fprintf( file, "\tbd.layers = %d;\n", m_layers );
	fprintf( file, "\tbd.allowSleep = bool( %d );\n", m_flags & eAllowSleep );
	fprintf( file, "\tbd.awake = bool( %d );\n", m_flags & eAwake );
	fprintf( file, "\tbd.awake = bool( %d );\n", m_flags & eAwake );
	fprintf( file, "\tbd.lockAxisX = bool( %d );\n", m_flags & eLockAxisX );
	fprintf( file, "\tbd.lockAxisY = bool( %d );\n", m_flags & eLockAxisY );
	fprintf( file, "\tbd.lockAxisZ = bool( %d );\n", m_flags & eLockAxisZ );
	fprintf( file, "\tbodies[ %d ] = scene.CreateBody( bd );\n\n", index );

	q3Box* box = m_boxes;

	while ( box )
	{
		fprintf( file, "\t{\n" );
		fprintf( file, "\t\tq3BoxDef sd;\n" );
		fprintf( file, "\t\tsd.SetFriction( r32( %.15lf ) );\n", box->friction );
		fprintf( file, "\t\tsd.SetRestitution( r32( %.15lf ) );\n", box->restitution );
		fprintf( file, "\t\tsd.SetDensity( r32( %.15lf ) );\n", box->density );
		i32 sensor = (int)box->sensor;
		fprintf( file, "\t\tsd.SetSensor( bool( %d ) );\n", sensor );
		fprintf( file, "\t\tq3Transform boxTx;\n" );
		q3Transform boxTx = box->local;
		q3Vec3 xAxis = boxTx.rotation.ex;
		q3Vec3 yAxis = boxTx.rotation.ey;
		q3Vec3 zAxis = boxTx.rotation.ez;
		fprintf( file, "\t\tq3Vec3 xAxis( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", xAxis.x, xAxis.y, xAxis.z );
		fprintf( file, "\t\tq3Vec3 yAxis( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", yAxis.x, yAxis.y, yAxis.z );
		fprintf( file, "\t\tq3Vec3 zAxis( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", zAxis.x, zAxis.y, zAxis.z );
		fprintf( file, "\t\tboxTx.rotation.SetRows( xAxis, yAxis, zAxis );\n" );
		fprintf( file, "\t\tboxTx.position.Set( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) );\n", boxTx.position.x, boxTx.position.y, boxTx.position.z );
		fprintf( file, "\t\tsd.Set( boxTx, q3Vec3( r32( %.15lf ), r32( %.15lf ), r32( %.15lf ) ) );\n", box->e.x * 2.0f, box->e.y * 2.0f, box->e.z * 2.0f );
		fprintf( file, "\t\tbodies[ %d ]->AddBox( sd );\n", index );
		fprintf( file, "\t}\n" );
		box = box->next;
	}

	fprintf( file, "}\n\n" );
}

//--------------------------------------------------------------------------------------------------
void q3Body::SynchronizeProxies( )
{
	q3BroadPhase* broadphase = &m_scene->m_contactManager.m_broadphase;

	// qiucw: m_localCenter is always (0,0,0)
	m_tx.position = m_worldCenter - q3Mul( m_tx.rotation, m_localCenter );

	q3AABB aabb;
	q3Transform tx = m_tx;

	q3Box* box = m_boxes;
	while ( box )
	{
		box->ComputeAABB( tx, &aabb );
		broadphase->Update( box->broadPhaseIndex, aabb );
		box = box->next;
	}
}
