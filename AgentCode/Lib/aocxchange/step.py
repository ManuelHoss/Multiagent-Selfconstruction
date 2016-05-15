#!/usr/bin/python
# coding: utf-8

r"""step module of aocxchange"""

from __future__ import print_function

import logging
import warnings

import OCC.BRep
import OCC.IFSelect
import OCC.Interface
import OCC.STEPControl
import OCC.TopoDS

import aocutils.types

import aocxchange.exceptions
import aocxchange.utils
import aocxchange.checks
import aocxchange.extensions

logger = logging.getLogger(__name__)


class StepImporter(object):
    r"""STEP file importer

    Parameters
    ----------
    filename : str

    """
    def __init__(self, filename=None):
        logger.info("StepImporter instantiated with filename : %s" % filename)
        self._shapes = list()
        self._number_of_shapes = 0

        aocxchange.checks.check_importer_filename(filename, aocxchange.extensions.step_extensions)

        self._filename = filename

        logger.info("Reading file ....")
        self.read_file()

    # CONFUSING !! Comes from an assignment in ReadFile but looks like the len of shapes
    # @property
    # def number_of_shapes(self):
    #     """ Number of shapes from the importer
    #
    #     Returns
    #     -------
    #     int
    #
    #     """
    #     return self._number_of_shapes

    def read_file(self):
        """
        Read the STEP file and stores the result in a _shapes list
        """
        stepcontrol_reader = OCC.STEPControl.STEPControl_Reader()
        status = stepcontrol_reader.ReadFile(self._filename)

        if status == OCC.IFSelect.IFSelect_RetDone:
            stepcontrol_reader.PrintCheckLoad(False, OCC.IFSelect.IFSelect_ItemsByEntity)
            nb_roots = stepcontrol_reader.NbRootsForTransfer()
            logger.info("%i root(s)" % nb_roots)
            if nb_roots == 0:
                msg = "No root for transfer"
                logger.error(msg)
                raise aocxchange.exceptions.StepFileReadException(msg)

            stepcontrol_reader.PrintCheckTransfer(False, OCC.IFSelect.IFSelect_ItemsByEntity)

            self._number_of_shapes = stepcontrol_reader.NbShapes()

            for n in range(1, nb_roots + 1):
                logger.info("Root index %i" % n)
                ok = stepcontrol_reader.TransferRoot(n)
                logger.info("TransferRoots status : %i" % ok)

                if ok:
                    # for i in range(1, self.nb_shapes + 1):
                    a_shape = stepcontrol_reader.Shape(n)
                    if a_shape.IsNull():
                        msg = "At least one shape in IGES cannot be transferred"
                        logger.warning(msg)
                    else:
                        self._shapes.append(a_shape)
                        logger.info("Appending a %s to list of shapes" %
                                    aocutils.types.topo_types_dict[a_shape.ShapeType()])
                else:
                    msg = "One shape could not be transferred"
                    logger.warning(msg)
                    warnings.warn(msg)
            return True
        else:
            msg = "Status is not OCC.IFSelect.IFSelect_RetDone"
            logger.error(msg)
            raise aocxchange.exceptions.StepFileReadException(msg)

    @property
    def compound(self):
        """ Create and returns a compound from the _shapes list"""
        # Create a compound
        compound = OCC.TopoDS.TopoDS_Compound()
        brep_builder = OCC.BRep.BRep_Builder()
        brep_builder.MakeCompound(compound)
        # Populate the compound
        for shape in self._shapes:
            brep_builder.Add(compound, shape)
        return compound

    @property
    def shapes(self):
        r"""Shapes

        Returns
        -------
        list[OCC.TopoDS.TopoDS_Shape]

        """
        return self._shapes


class StepExporter(object):
    r"""STEP file exporter

    Parameters
    ----------
    filename : str
        the file to save to eg. myshape.step
    verbose : bool
        verbosity of the STEP exporter
    schema : ["AP203", "AP214CD"]
        which STEP schema to use, either AP214CD or AP203
    tolerance : float

    """
    def __init__(self, filename, verbose=False, schema="AP214CD", tolerance=1e-4):
        logger.info("StepExporter instantiated with filename : %s" % filename)
        logger.info("StepExporter schema : %s" % schema)
        logger.info("StepExporter tolerance : %s" % str(tolerance))

        if schema not in ["AP203", "AP214CD"]:
            msg = "Unsupported STEP schema"
            logger.error(msg)
            raise aocxchange.exceptions.StepUnknownSchemaException(msg)

        aocxchange.checks.check_exporter_filename(filename, aocxchange.extensions.step_extensions)
        aocxchange.checks.check_overwrite(filename)

        self._filename = filename
        self._shapes = list()
        self.verbose = verbose

        self._stepcontrol_writer = OCC.STEPControl.STEPControl_Writer()
        self._stepcontrol_writer.SetTolerance(tolerance)

        OCC.Interface.Interface_Static_SetCVal("write.step.schema", schema)

    def add_shape(self, a_shape):
        r"""Add a shape to export

        Parameters
        ----------
        a_shape : TopoDS_Shape or subclass

        """
        aocxchange.checks.check_shape(a_shape)  # raises an exception if the shape is not valid
        self._shapes.append(a_shape)

    def write_file(self):
        r"""Write STEP file"""
        for shp in self._shapes:
            transfer_status = self._stepcontrol_writer.Transfer(shp, OCC.STEPControl.STEPControl_AsIs)
            if transfer_status != OCC.IFSelect.IFSelect_RetDone:
                msg = "An error occurred while transferring a shape to the STEP writer"
                logger.error(msg)
                raise aocxchange.exceptions.StepShapeTransferException(msg)

        write_status = self._stepcontrol_writer.Write(self._filename)

        if self.verbose:
            self._stepcontrol_writer.PrintStatsTransfer()

        if write_status == OCC.IFSelect.IFSelect_RetDone:
            logger.info("STEP file write successful.")
        else:
            msg = "An error occurred while writing the STEP file"
            logger.error(msg)
            raise aocxchange.exceptions.StepFileWriteException(msg)
