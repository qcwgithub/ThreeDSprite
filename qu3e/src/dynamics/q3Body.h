//--------------------------------------------------------------------------------------------------
/**
@file	q3Body.h

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

#ifndef Q3BODY_H
#define Q3BODY_H

#include <stdio.h>

#include "../math/q3Math.h"
#include "../math/q3Transform.h"

//--------------------------------------------------------------------------------------------------
// q3Body
//--------------------------------------------------------------------------------------------------
class q3Scene;
struct q3BodyDef;
class q3BoxDef;
struct q3ContactEdge;
class q3Render;
struct q3Box;

enum q3BodyType
{
	eStaticBody,
	eDynamicBody,
	eKinematicBody
};

class q3Body
{
public:

	// Adds a box to this body. Boxes are all defined in local space
	// of their owning body. Boxes cannot be defined relative to one
	// another. The body will recalculate its mass values. No contacts
	// will be created until the next q3Scene::Step( ) call.
	const q3Box* AddBox( const q3BoxDef& def );

	// Removes this box from the body and broadphase. Forces the body
	// to recompute its mass if the body is dynamic. Frees the memory
	// pointed to by the box pointer.
	void RemoveBox( const q3Box* box );

	// Removes all boxes from this body and the broadphase.
	void RemoveAllBoxes( );
	void SetToAwake( );
	void SetToSleep( );
	bool IsAwake( ) const;
	const q3Vec3 GetLocalPoint( const q3Vec3& p ) const;
	const q3Vec3 GetLocalVector( const q3Vec3& v ) const;
	const q3Vec3 GetWorldPoint( const q3Vec3& p ) const;
	const q3Vec3 GetWorldVector( const q3Vec3& v ) const;

	bool CanCollide( const q3Body *other ) const;
	const q3Transform GetTransform( ) const;
	i32 GetFlags( ) const;
	void SetLayers( i32 layers );
	i32 GetLayers( ) const;
	const q3Quaternion GetQuaternion( ) const;
	void* GetUserData( ) const;	

	// Manipulating the transformation of a body manually will result in
	// non-physical behavior. Contacts are updated upon the next call to
	// q3Scene::Step( ). Parameters are in world space. All body types
	// can be updated.
	void SetPosition( const q3Vec3& position );
	void SetRotation(const q3Quaternion& q);
	void SetTransform(const q3Vec3& position, const q3Quaternion& q);

	// Used for debug rendering lines, triangles and basic lighting
	void Render( q3Render* render ) const;

	// Dump this rigid body and its shapes into a log file. The log can be
	// used as C++ code to re-create an initial scene setup.
	void Dump( FILE* file, i32 index ) const;
private:
	// m_flags
	enum
	{
		eAwake		= 0x001,
		eActive		= 0x002,
		eAllowSleep	= 0x004,
		eIsland		= 0x010,
		eStatic		= 0x020,
		eDynamic	= 0x040,
		eKinematic	= 0x080,
		eLockAxisX	= 0x100,
		eLockAxisY	= 0x200,
		eLockAxisZ	= 0x400,
	};

	q3Transform m_tx;
	q3Quaternion m_q;
	q3Vec3 m_localCenter;
	q3Vec3 m_worldCenter;
	r32 m_sleepTime;
	i32 m_layers;
	i32 m_flags;

	q3Box* m_boxes;
	void *m_userData;
	q3Scene* m_scene;
	q3Body* m_next;
	q3Body* m_prev;

	q3ContactEdge* m_contactList;

	friend class q3Scene;
	friend struct q3Manifold;
	friend class q3ContactManager;

	q3Body( const q3BodyDef& def, q3Scene* scene );

	void SynchronizeProxies( );
};

//--------------------------------------------------------------------------------------------------
// q3BodyDef
//--------------------------------------------------------------------------------------------------
struct q3BodyDef
{
	q3BodyDef( )
	{
		// Set all initial positions/velocties to zero
		q3Identity( axis );
		angle = r32( 0.0 );
		q3Identity( position );

		// Common default values
		bodyType = eStaticBody;
		layers = 0x000000001;
		userData = NULL;
		allowSleep = true;
		awake = true;
		active = true;
		lockAxisX = false;
		lockAxisY = false;
		lockAxisZ = false;
	}

	q3Vec3 axis;			// Initial world transformation.
	r32 angle;				// Initial world transformation. Radians.
	q3Vec3 position;		// Initial world transformation.
	i32 layers;				// Bitmask of collision layers. Bodies matching at least one layer can collide.
	void* userData;			// Use to store application specific data.

	// Static, dynamic or kinematic. Dynamic bodies with zero mass are defaulted
	// to a mass of 1. Static bodies never move or integrate, and are very CPU
	// efficient. Static bodies have infinite mass. Kinematic bodies have
	// infinite mass, but *do* integrate and move around. Kinematic bodies do not
	// resolve any collisions.
	q3BodyType bodyType;

	bool allowSleep;	// Sleeping lets a body assume a non-moving state. Greatly reduces CPU usage.
	bool awake;			// Initial sleep state. True means awake.
	bool active;		// A body can start out inactive and just sits in memory.
	bool lockAxisX;		// Locked rotation on the x axis.
	bool lockAxisY;		// Locked rotation on the y axis.
	bool lockAxisZ;		// Locked rotation on the z axis.
};

#endif // Q3BODY_H
