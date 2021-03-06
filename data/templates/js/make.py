# {{{autogeneratedwarning}}}

from __future__ import print_function

"""Builds Cubism Core JavaScript bindings."""


# User options.
USER_OPTIONS = {
    'allowmemorygrowth': True,
    'uglify': True
}


if __name__ == '__main__':
    import os
    import shutil
    import sys
    from subprocess import call


    exportedfunctionslist = [
        {{#funcs}}
        '{{{entry}}}',
        {{/funcs}}
        None
    ]
    selfdir = os.path.dirname(os.path.realpath(__file__))
    indir = os.path.join(selfdir, '.in')
    outdir = os.path.join(selfdir, 'out')
    tempdir = os.path.join(selfdir, '.temp')
    if not os.path.exists(outdir):
        os.makedirs(outdir)
    if not os.path.exists(tempdir):
        os.makedirs(tempdir)
    coredir = sys.argv[sys.argv.index('--coredir') + 1] if '--coredir' in sys.argv else None
    if not coredir:
        print('ERROR: Please specify path to native Cubism Core directory with \'--coredir\' option and retry.')
        sys.exit(1)


    # Assemble Emscripten command...
    emcmd = ['emcc', '-O3', '--memory-init-file', '0', '-s', 'MODULARIZE=1', '-s']
    if USER_OPTIONS['allowmemorygrowth']:
        emcmd.append('ALLOW_MEMORY_GROWTH=1')
    else:
        emcmd.append('ALLOW_MEMORY_GROWTH=0')
    emcmd.append('-s')
    emcmd.append('EXPORT_NAME="_em_module"')
    exportedfunctions = 'EXPORTED_FUNCTIONS=['
    for func in exportedfunctionslist:
        if not func:
            continue
        exportedfunctions += ('\'_' + func + '\', ')
    exportedfunctions = exportedfunctions[:-2] + ']'
    emcmd.append('-s')
    emcmd.append(exportedfunctions)
    emcmd.append('-I' + os.path.join(coredir, 'include'))
    emcmd.append('-o')
    emcmd.append(os.path.join(tempdir, '_em_module.js'))
    emcmd.append(os.path.join(coredir, 'lib', 'experimental', 'emscripten', 'Live2DCubismCore.bc'))
    emcmd.append(os.path.join(indir, 'Live2DCubismCoreEMSCRIPTEN.c'))
    emcmd.append('-s')
    emcmd.append('EXTRA_EXPORTED_RUNTIME_METHODS=[''ccall'', ''Pointer_stringify'']')
    emcmd.append('-s')
    emcmd.append('WASM=0')

    # ... and execute it.
    if os.name == 'nt':
        call(emcmd, shell=True)
    else:
        call(emcmd)

    # Assemble and execute TypeScript command.
    tsccmd = ['tsc', os.path.join(indir, 'live2dcubismcore.ts'), '--outDir', outdir, '-D', '--sourceMap']
    # Execute TypeScript command.
    if os.name == 'nt':
        call(tsccmd, shell=True)
    else:
        call(tsccmd)

    # Merge JavaScript with Emscripten module.
    with open(os.path.join(outdir, 'live2dcubismcore.js')) as jsstream, open(os.path.join(tempdir, '_em_module.js')) as emstream: 
        js = jsstream.read()
        em = emstream.read()

    with open(os.path.join(outdir, 'live2dcubismcore.js'), 'w') as jsstream:
        jsstream.write(js.replace('///\\\\\\_em_module///\\\\\\', em + 'var _em = _em_module();\n'))


    # Uglify if requested.
    if USER_OPTIONS['uglify']:
        # Assemble command and execute it.
        uglifycmd = ['uglifyjs', '--keep-fnames', '-o', os.path.join(outdir, 'live2dcubismcore.min.js'), os.path.join(outdir, 'live2dcubismcore.js')]
        
        if os.name == 'nt':
            call(uglifycmd, shell=True)
        else:
            call(uglifycmd)

    # Delete temporary directory.
    shutil.rmtree(tempdir, True)
