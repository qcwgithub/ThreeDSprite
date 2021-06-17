//--------------------------------------------------------------------------------------------------
/**
@file	q3Scene.h

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

#include <stdlib.h>

#include "q3Scene.h"
#include "../dynamics/q3Body.h"
#include "../dynamics/q3Contact.h"
#include "../collision/q3Box.h"

//--------------------------------------------------------------------------------------------------
// q3Scene
//--------------------------------------------------------------------------------------------------
q3Scene::q3Scene( )
	: m_contactManager( &m_stack )
	, m_boxAllocator( sizeof( q3Box ), 256 )
	, m_bodyCount( 0 )
	, m_bodyList( NULL )
	, m_newBox( false )
	, m_allowSleep( true )
{
}

//--------------------------------------------------------------------------------------------------
q3Scene::~q3Scene( )
{
	Shutdown( );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::Step( )
{
	if ( m_newBox )
	{
		m_contactManager.m_broadphase.UpdatePairs( );
		m_newBox = false;
	}

	m_contactManager.TestCollisions( );
}

//--------------------------------------------------------------------------------------------------
q3Body* q3Scene::CreateBody( const q3BodyDef& def )
{
	q3Body* body = (q3Body*)m_heap.Allocate( sizeof( q3Body ) );
	new (body) q3Body( def, this );

	// Add body to scene bodyList
	body->m_prev = NULL;
	body->m_next = m_bodyList;

	if ( m_bodyList )
		m_bodyList->m_prev = body;

	m_bodyList = body;
	++m_bodyCount;

	return body;
}

//--------------------------------------------------------------------------------------------------
void q3Scene::RemoveBody( q3Body* body )
{
	assert( m_bodyCount > 0 );

	m_contactManager.RemoveContactsFromBody( body );

	body->RemoveAllBoxes( );

	// Remove body from scene bodyList
	if ( body->m_next )
		body->m_next->m_prev = body->m_prev;

	if ( body->m_prev )
		body->m_prev->m_next = body->m_next;

	if ( body == m_bodyList )
		m_bodyList = body->m_next;

	--m_bodyCount;

	m_heap.Free( body );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::RemoveAllBodies( )
{
	q3Body* body = m_bodyList;

	while ( body )
	{
		q3Body* next = body->m_next;

		body->RemoveAllBoxes( );

		m_heap.Free( body );

		body = next;
	}

	m_bodyList = NULL;
}

//--------------------------------------------------------------------------------------------------
void q3Scene::SetAllowSleep( bool allowSleep )
{
	m_allowSleep = allowSleep;

	if ( !allowSleep )
	{
		for ( q3Body* body = m_bodyList; body; body = body->m_next )
			body->SetToAwake( );
	}
}

//--------------------------------------------------------------------------------------------------
void q3Scene::Render( q3Render* render ) const
{
	q3Body* body = m_bodyList;

	while ( body )
	{
		body->Render( render );
		body = body->m_next;
	}

	m_contactManager.RenderContacts( render );
	//m_contactManager.m_broadphase.m_tree.Render( render );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::Shutdown( )
{
	RemoveAllBodies( );

	m_boxAllocator.Clear( );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::SetContactListener( q3ContactListener* listener )
{
	m_contactManager.m_contactListener = listener;
}
q3ContactListener* q3Scene::GetContactListener()
{
	return m_contactManager.m_contactListener;
}

//--------------------------------------------------------------------------------------------------
void q3Scene::QueryAABB( q3QueryCallback *cb, const q3AABB& aabb ) const
{
	struct SceneQueryWrapper
	{
		bool TreeCallBack( i32 id )
		{
			q3AABB aabb;
			q3Box *box = (q3Box *)broadPhase->m_tree.GetUserData( id );

			box->ComputeAABB( box->body->GetTransform( ), &aabb );

			if ( q3AABBtoAABB( m_aabb, aabb ) )
			{
				return cb->ReportShape( box );
			}

			return true;
		}

		q3QueryCallback *cb;
		const q3BroadPhase *broadPhase;
		q3AABB m_aabb;
	};

	SceneQueryWrapper wrapper;
	wrapper.m_aabb = aabb;
	wrapper.broadPhase = &m_contactManager.m_broadphase;
	wrapper.cb = cb;
	m_contactManager.m_broadphase.m_tree.Query( &wrapper, aabb );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::QueryPoint( q3QueryCallback *cb, const q3Vec3& point ) const
{
	struct SceneQueryWrapper
	{
		bool TreeCallBack( i32 id )
		{
			q3Box *box = (q3Box *)broadPhase->m_tree.GetUserData( id );

			if ( box->TestPoint( box->body->GetTransform( ), m_point ) )
			{
				cb->ReportShape( box );
			}

			return true;
		}

		q3QueryCallback *cb;
		const q3BroadPhase *broadPhase;
		q3Vec3 m_point;
	};

	SceneQueryWrapper wrapper;
	wrapper.m_point = point;
	wrapper.broadPhase = &m_contactManager.m_broadphase;
	wrapper.cb = cb;
	const r32 k_fattener = r32( 0.5 );
	q3Vec3 v( k_fattener, k_fattener, k_fattener );
	q3AABB aabb;
	aabb.min = point - v;
	aabb.max = point + v;
	m_contactManager.m_broadphase.m_tree.Query( &wrapper, aabb );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::RayCast( q3QueryCallback *cb, q3RaycastData& rayCast ) const
{
	struct SceneQueryWrapper
	{
		bool TreeCallBack( i32 id )
		{
			q3Box *box = (q3Box *)broadPhase->m_tree.GetUserData( id );

			if ( box->Raycast( box->body->GetTransform( ), m_rayCast ) )
			{
				return cb->ReportShape( box );
			}

			return true;
		}

		q3QueryCallback *cb;
		const q3BroadPhase *broadPhase;
		q3RaycastData *m_rayCast;
	};
	
	SceneQueryWrapper wrapper;
	wrapper.m_rayCast = &rayCast;
	wrapper.broadPhase = &m_contactManager.m_broadphase;
	wrapper.cb = cb;
	m_contactManager.m_broadphase.m_tree.Query( &wrapper, rayCast );
}

//--------------------------------------------------------------------------------------------------
void q3Scene::Dump( FILE* file ) const
{
	fprintf( file, "// Ensure 64/32-bit memory compatability with the dump contents\n" );
	fprintf( file, "assert( sizeof( int* ) == %lu );\n", sizeof( int* ) );
	fprintf( file, "scene.SetAllowSleep( %s );\n", m_allowSleep ? "true" : "false" );

	fprintf( file, "q3Body** bodies = (q3Body**)q3Alloc( sizeof( q3Body* ) * %d );\n", m_bodyCount );

	i32 i = 0;
	for ( q3Body* body = m_bodyList; body; body = body->m_next, ++i )
	{
		body->Dump( file, i );
	}

	fprintf( file, "q3Free( bodies );\n" );
}

typedef void(*ContactDelegate)(q3Body *bodyA, q3Box *boxA, q3Body *bodyB, q3Box *boxB);

class MyContactListener : public q3ContactListener
{
public:
	MyContactListener()
	{
		m_onBeginContact = 0;
		m_onEndContact = 0;
	}
	ContactDelegate m_onBeginContact;
	ContactDelegate m_onEndContact;

	virtual void BeginContact(const q3ContactConstraint *contact)
	{
		if (m_onBeginContact)
		{
			m_onBeginContact(contact->bodyA, contact->A, contact->bodyB, contact->B);
		}
	}
	virtual void EndContact(const q3ContactConstraint *contact)
	{
		if (m_onEndContact)
		{
			m_onEndContact(contact->bodyA, contact->A, contact->bodyB, contact->B);
		}
	}
};

extern "C" __declspec(dllexport)
q3Scene* CreateScene()
{
	q3Scene *scene = new q3Scene();
	scene->SetContactListener(new MyContactListener());
	return scene;
}

extern "C" __declspec(dllexport)
void SceneDestroy(q3Scene *scene)
{
	delete scene;
}

extern "C" __declspec(dllexport)
void SceneSetContactListener(q3Scene *scene, ContactDelegate onBegin, ContactDelegate onEnd)
{
	MyContactListener *listener = (MyContactListener *)scene->GetContactListener();
	listener->m_onBeginContact = onBegin;
	listener->m_onEndContact = onEnd;
}

extern "C" __declspec(dllexport)
void SceneStep(q3Scene *scene)
{
	scene->Step();
}

extern "C" __declspec(dllexport)
q3Body* SceneAddBody(q3Scene *scene, q3BodyType bodyType, r32 x, r32 y, r32 z)
{
	q3BodyDef def;
	def.position.Set(x, y, z);
	def.bodyType = bodyType;
	q3Body *body = scene->CreateBody(def);
	return body;
}

extern "C" __declspec(dllexport)
void BodySetToAwake(q3Body *body)
{
	body->SetToAwake();
}

extern "C" __declspec(dllexport)
void BodySetToSleep(q3Body *body)
{
	body->SetToSleep();
}

extern "C" __declspec(dllexport)
void BodySetPosition(q3Body *body, r32 x, r32 y, r32 z)
{
	q3Vec3 pos(x, y, z);
	body->SetTransform(pos);
}

extern "C" __declspec(dllexport)
const q3Box* BodyAddBox(q3Body *body, r32 x, r32 y, r32 z, r32 extend_x, r32 extend_y, r32 extend_z)
{
	q3Transform tx;
	q3Identity(tx);
	tx.position.Set(x, y, z);

	q3BoxDef def;
	def.Set(tx, q3Vec3(extend_x, extend_y, extend_z));

	const q3Box *box = body->AddBox(def);
	return box;
}