﻿using CourseSeller.Core.Services.Interfaces;
using CourseSeller.DataLayer.Contexts;
using CourseSeller.DataLayer.Entities.Permissions;
using Microsoft.EntityFrameworkCore;

namespace CourseSeller.Core.Services;

public class PermissionService : IPermissionService
{
    private readonly MssqlContext _context;

    public PermissionService(MssqlContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>> GetAll()
    {
        return await _context.Permission.ToListAsync();
    }

    public async Task AddPermissionsToRole(int roleId, List<int> permissions)
    {
        var rolePermission = new List<RolePermission>();
        foreach (var p in permissions)
        {
            rolePermission.Add(new RolePermission()
            {
                RoleId = roleId,
                PermissionId = p,
            });
        }

        await _context.RolePermission.AddRangeAsync(rolePermission);
        await _context.SaveChangesAsync();
    }

    public async Task<List<int>> GetRoleSelectedPermissionsId(int roleId)
    {
        return await _context.RolePermission
            .Where(r => r.RoleId == roleId)
            .Select(r => r.PermissionId)
            .ToListAsync();
    }

    public async Task UpdateRolePermission(int roleId, List<int> permissions)
    {
        var rolePermissions = await _context.RolePermission
                                                .Where(rp => rp.RoleId == roleId)
                                                .ToListAsync();
        _context.RolePermission.RemoveRange(rolePermissions);
        await AddPermissionsToRole(roleId, permissions);
    }
}

